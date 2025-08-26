using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Core.Interfaces.Service
{
    public interface IProdutoService : IBaseService<Produto>
    {
        Task<Produto?> GetByNomeAsync(string email);
        Task NovoProdutoAsync(ProdutoRequest request);
        Task AtualizarProdutoAsync(ProdutoRequest request);
        Task AtualizarProdutoAdminAsync(ProdutoAdminRequest request);
    }
}