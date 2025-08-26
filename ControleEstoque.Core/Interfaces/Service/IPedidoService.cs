using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Core.Interfaces.Service
{
    public interface IPedidoService : IBaseService<Pedido>
    {
        Task NovoPedidoAsync(PedidoRequest request);
    }
}