using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Infrastructure.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        private readonly ControleEstoqueDbContext _context;

        public ProdutoRepository(ControleEstoqueDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Produto?> GetByNomeAsync(string nome)
        {
            return await _context.Produtos.FirstOrDefaultAsync(u => u.Nome == nome);
        }
    }
}