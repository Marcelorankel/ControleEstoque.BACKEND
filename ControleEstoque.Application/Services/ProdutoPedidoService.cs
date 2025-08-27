using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using Elastic.Apm;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using static ControleEstoque.Application.Middlewares.ErrorHandlingMiddleware;

namespace ControleEstoque.Application.Services
{
    public class ProdutoPedidoService : BaseService<ProdutoPedido>, IProdutoPedidoService
    {
        private readonly IProdutoPedidoRepository _produtoPedidoRepository;

        public ProdutoPedidoService(IProdutoPedidoRepository produtoPedidoRepository, TracerProvider tracerProvider)
            : base(produtoPedidoRepository)  // passa pro BaseService
        {
            _produtoPedidoRepository = produtoPedidoRepository;
        }       
    }
}