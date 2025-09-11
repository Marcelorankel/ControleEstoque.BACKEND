using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using ControleEstoque.Infrastructure.Persistence;
using MongoDB.Bson;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace ControleEstoque.WorkerService
{
    public class WorkerControleEstoque : BackgroundService
    {
        private readonly ILogger<WorkerControleEstoque> _logger;
        private readonly ConnectionFactory _factory;
        private readonly IServiceScopeFactory _scopeFactory;

        public WorkerControleEstoque(
            ILogger<WorkerControleEstoque> logger,
            IServiceScopeFactory scopeFactory
            )
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _factory = new ConnectionFactory()
            {
                HostName =
                             //Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_DOCKER") == "true"
                             //            ? 
                             "rabbitmq"
                //"localhost"
                //: "localhost"
            }; // RabbitMQ
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker rodando at: {time}", DateTimeOffset.Now);

            Console.WriteLine("Consumindo msgs fila pedidos");

            // cria conexão e canal apenas uma vez
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: "messages",
                type: ExchangeType.Direct,
                durable: true
            );

            await channel.QueueDeclareAsync(
                queue: "pedidos",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            await channel.QueueBindAsync("pedidos", "messages", string.Empty);

            Console.WriteLine("Aguardando mensagens...");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var _pedidoRepo = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();
                var _produtoRepo = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();
                var _produtoPedidoRepo = scope.ServiceProvider.GetRequiredService<IProdutoPedidoRepository>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ControleEstoqueDbContext>();

                using var transaction = await dbContext.Database.BeginTransactionAsync();

                byte[] body = eventArgs.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"MSG Recebida : {message}");

                var request = JsonSerializer.Deserialize<PedidoRabbitMQResponse>(message);

                try
                {
                    var objPedido = new Pedido
                    {
                        Id = Guid.NewGuid(),
                        IdUsuario = request.IdUsuario,
                        DocumentoCliente = request.DocumentoCliente,
                        DataPedido = request.DataPedido,
                        ValorTotal = request.ValorTotal,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _pedidoRepo.CreateAsync(objPedido);

                    foreach (var item in request.Produtos)
                    {
                        var auxProdutoPedido = new ProdutoPedido
                        {
                            Id = Guid.NewGuid(),
                            IdProduto = item.Id,
                            IdPedido = objPedido.Id,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _produtoPedidoRepo.CreateAsync(auxProdutoPedido);
                    }

                    foreach (var item in request.Produtos)
                    {
                        var aux = await _produtoRepo.GetByIdAsync(item.Id);
                        aux.Quantidade -= item.Quantidade;
                        await _produtoRepo.UpdateAsync(aux);
                    }

                    await transaction.CommitAsync();

                    await ((AsyncEventingBasicConsumer)sender)
                        .Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem do pedido");
                    await transaction.RollbackAsync();

                    await ((AsyncEventingBasicConsumer)sender)
                        .Channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await channel.BasicConsumeAsync("pedidos", autoAck: false, consumer);

            // mantém o Worker vivo até receber stop
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}