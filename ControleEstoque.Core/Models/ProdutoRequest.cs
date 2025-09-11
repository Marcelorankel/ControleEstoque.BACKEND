using ControleEstoque.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Core.Models
{
    public class ProdutoRequest
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public decimal Preco { get; set; }
    }

    public class ProdutoAdminRequest : ProdutoRequest
    {
        public int Quantidade { get; set; }
    }

    public class ProdutoCadRequest
    {
        public string Nome { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public decimal Preco { get; set; }
    }
}