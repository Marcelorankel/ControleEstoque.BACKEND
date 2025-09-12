using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
//using Elastic.Apm;
//using Elastic.Apm.Api;
//using OpenTelemetry.Trace;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using static ControleEstoque.Application.Middlewares.ErrorHandlingMiddleware;

namespace ControleEstoque.Application.Services
{
    public class PedidoService : BaseService<Pedido>, IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProdutoRepository _produtoRepository;
        //private readonly Tracer _tracer;
        private readonly ConnectionFactory _factory;

        public PedidoService(IPedidoRepository pedidoRepository, 
            IUsuarioRepository usuarioRepository, 
            IProdutoRepository produtoRepository,
            ConnectionFactory factory
            //, TracerProvider tracerProvider
            )
            : base(pedidoRepository)  // passa pro BaseService
        {
            _pedidoRepository = pedidoRepository;
            _usuarioRepository = usuarioRepository;
            _produtoRepository = produtoRepository;
            //_tracer = tracerProvider.GetTracer("PedidoService");
            _factory = factory;
        }

        public async Task NovoPedidoFilaAsync(PedidoRequest request)
        {
            ////Trace
            //using var span = _tracer.StartActiveSpan("CriarPedido");

            ////Elastic APM
            //var transaction = Agent.Tracer.CurrentTransaction;
            //var elasticSpan = transaction?.StartSpan("CriarPedido", "custom");

            //Validadores
            //DataPedido
            if (request.DataPedido == DateTime.MinValue)
                throw new ValidationException($"Data Pedido invalida");
            //DocumentoCliente
            if (request.DocumentoCliente == string.Empty)
                throw new ValidationException($"Documento cliente não informado");
            //IdUsuario
            if (request.IdUsuario == Guid.Empty)
                throw new ValidationException($"Id Usuario não informado");


            decimal auxVotal = 0;
            //Valida Lista de Produtos
            foreach (var item in request.Produtos)
            {
                //Soma valor total pedido
                auxVotal += item.Preco * item.Quantidade;

                var aux = await _produtoRepository.GetByIdAsync(item.Id);
                if (aux == null)
                    throw new ValidationException($"Produto ID - {item.Id} não foi encontrado");
                if (item.Quantidade > aux.Quantidade) //Valida quantidade produto estoque disponivel
                    throw new ValidationException($"Quantidade disponivel no estoque do produto solicitado ID - {item.Id} Nome - {item.Nome} é de {aux.Quantidade}");
            }
            try
            {
                //Busca usuario no banco por ID
                var res = await _usuarioRepository.GetByIdAsync(request.IdUsuario);
                if (res == null)
                    throw new ValidationException($"Usuario ID - {request.IdUsuario} não foi encontrado");

                var obj = new PedidoRabbitMQResponse
                {
                    IdUsuario = request.IdUsuario,
                    DocumentoCliente = request.DocumentoCliente,
                    DataPedido = request.DataPedido,
                    ValorTotal = auxVotal,
                    Produtos = request.Produtos,
                    Usuario = res
                };

                //Envia fila RabbitMQ
                using var connection = await _factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                // Declaração da fila
                await channel.QueueDeclareAsync(
                    queue: "pedidos",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Serializa e publica a mensagem
                var json = JsonSerializer.Serialize(obj);
                var body = Encoding.UTF8.GetBytes(json);

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "pedidos",
                    mandatory: false,
                    basicProperties: new BasicProperties { Persistent = true },
                    body: body);

                Console.WriteLine("Pedido enviado para fila RabbitMQ!");

                //span.SetAttribute("pedido.enviadoFila", request?.ToString() ?? "0");
                //elasticSpan?.SetLabel("pedido.enviadoFila", request?.ToString() ?? "0");
            }
            finally
            {
                //elasticSpan?.End();
            }
        }

        public async Task<PedidoResponse> ObterPedidoPorIdAsync(Guid id)
        {
            return await _pedidoRepository.ObterPedidoPorIdAsync(id);
        }
        public async Task<List<PedidoResponse>> ObterPedidosPorEmailUsuarioAsync(string email)
        {
            return await _pedidoRepository.ObterPedidosPorEmailUsuarioAsync(email);
        }

        public async Task<List<PedidoResponse>> ObterTodosPedidosAsync()
        {
            return await _pedidoRepository.ObterTodosPedidosAsync();
        }
    }
}