using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;

namespace ControleEstoque.Application.Services
{
    public class ProdutoPedidoService : BaseService<ProdutoPedido>, IProdutoPedidoService
    {
        private readonly IProdutoPedidoRepository _produtoPedidoRepository;

        public ProdutoPedidoService(IProdutoPedidoRepository produtoPedidoRepository
            )
            : base(produtoPedidoRepository)  // passa pro BaseService
        {
            _produtoPedidoRepository = produtoPedidoRepository;
        }       
    }
}