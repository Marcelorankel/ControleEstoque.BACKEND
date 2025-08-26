using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Core.Models
{
    public class PedidoRequest
    {
        public Guid IdUsuario { get; set; }
        public string? DocumentoCliente { get; set; }
        public DateTime DataPedido { get; set; }
        public decimal ValorTotal { get; set; }

        public ICollection<ProdutoAdminRequest> Produtos { get; set; }
    }
}