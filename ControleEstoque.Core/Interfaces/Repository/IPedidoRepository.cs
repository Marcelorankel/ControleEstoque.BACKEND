using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Core.Interfaces.Repository
{
    public interface IPedidoRepository : IBaseRepository<Pedido>
    {
        Task<PedidoResponse> ObterPedidoPorIdAsync(Guid idPedido);
        Task<List<PedidoResponse>> ObterPedidosPorEmailUsuarioAsync(string email);
        Task<List<PedidoResponse>> ObterTodosPedidosAsync();
    }
}