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
            _factory = new ConnectionFactory() { HostName = "localhost" }; // RabbitMQ
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var x = Guid.NewGuid();
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {                  
                    _logger.LogInformation("Worker rodando at: {time}", DateTimeOffset.Now);

                    Console.WriteLine("Consumindo msgs fila pedidos");
                    using var connection = await _factory.CreateConnectionAsync();
                    using var channel = await connection.CreateChannelAsync();

                    await channel.QueueDeclareAsync(
                            queue: "pedidos",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                    await channel.QueueBindAsync("pedidos", "messages", string.Empty);

                    Console.WriteLine("Aguardando mensagens...");

                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.ReceivedAsync += async (sender, eventArgs) =>
                    {
                        // Cria escopo para usar serviços Scoped
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
                                ProdutosPedidos = null,
                                CreatedAt = DateTime.UtcNow
                            };


                            var resultPedido = await _pedidoRepo.CreateAsync(objPedido);

                            //Insere items do pedido na tabela ProdutoPedido
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

                            //Reduz estoque dos produtos do pedido
                            foreach (var item in request.Produtos)
                            {
                                var aux = await _produtoRepo.GetByIdAsync(item.Id);
                                aux.Quantidade -= item.Quantidade;

                                await _produtoRepo.UpdateAsync(aux);
                            }

                            //Confirma transação no banco
                            await transaction.CommitAsync();
                            //Confirma mensagem no RabbitMQ
                            await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erro ao processar mensagem do pedido");

                            await transaction.RollbackAsync();

                            // Nack para reprocessar depois ou mover para DLQ
                            await ((AsyncEventingBasicConsumer)sender).Channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                        }
                    };

                    await channel.BasicConsumeAsync("pedidos", autoAck: false, consumer);

                    Console.ReadLine();
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
