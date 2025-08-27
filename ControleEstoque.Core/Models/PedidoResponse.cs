using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Core.Models
{
    public class PedidoResponse
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }
        public string DocumentoCliente { get; set; }
        public DateTime DataPedido { get; set; }
        public decimal ValorTotal { get; set; }
        public List<ProdutoResponse> Produtos { get; set; } = new List<ProdutoResponse>();
    }
}