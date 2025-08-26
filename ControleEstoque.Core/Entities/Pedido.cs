using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Core.Entities
{
    public class Pedido :BaseEntity
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }   // FK
        public string? DocumentoCliente { get; set; }
        public DateTime DataPedido { get; set; }
        public decimal ValorTotal { get; set; }

        // Navegação
        public Usuario Usuario { get; set; }
    }
}