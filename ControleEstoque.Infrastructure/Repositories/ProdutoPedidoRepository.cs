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
    public class ProdutoPedidoRepository : BaseRepository<ProdutoPedido>, IProdutoPedidoRepository
    {
        private readonly ControleEstoqueDbContext _context;

        public ProdutoPedidoRepository(ControleEstoqueDbContext context) : base(context)
        {
            _context = context;
        }
    }
}