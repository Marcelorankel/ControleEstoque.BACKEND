using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Models;
using ControleEstoque.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Infrastructure.Repositories
{
    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {
        private readonly ControleEstoqueDbContext _context;

        public PedidoRepository(ControleEstoqueDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PedidoResponse> ObterPedidoPorIdAsync(Guid idPedido)
        {
            return await _context.Pedidos
            .Where(p => p.Id == idPedido && p.DeletedAt == null)
            .Select(p => new PedidoResponse
            {
                Id = p.Id,
                IdUsuario = p.IdUsuario,
                DocumentoCliente = p.DocumentoCliente,
                DataPedido = p.DataPedido,
                ValorTotal = p.ValorTotal,
                Produtos = p.ProdutosPedidos.Select(pp => new ProdutoResponse
                {
                    Id = pp.Produto.Id,
                    Nome = pp.Produto.Nome,
                    Preco = pp.Produto.Preco
                }).ToList()
            })
            .FirstOrDefaultAsync();
        }

        public async Task<List<PedidoResponse>> ObterPedidosPorEmailUsuarioAsync(string email)
        {
            return await _context.Pedidos
                    .Where(p => p.Usuario.Email == email)
                    .Select(p => new PedidoResponse
                    {
                        Id = p.Id,
                        IdUsuario = p.IdUsuario,
                        DocumentoCliente = p.DocumentoCliente,
                        DataPedido = p.DataPedido,
                        ValorTotal = p.ValorTotal,
                        Produtos = p.ProdutosPedidos.Select(pp => new ProdutoResponse
                        {
                            Id = pp.Produto.Id,
                            Nome = pp.Produto.Nome,
                            Preco = pp.Produto.Preco
                        }).ToList()
                    })
                    .ToListAsync();
        }

        public async Task<List<PedidoResponse>> ObterTodosPedidosAsync()
        {
            return await _context.Pedidos
                    .Select(p => new PedidoResponse
                    {
                        Id = p.Id,
                        IdUsuario = p.IdUsuario,
                        DocumentoCliente = p.DocumentoCliente,
                        DataPedido = p.DataPedido,
                        ValorTotal = p.ValorTotal,
                        Produtos = p.ProdutosPedidos.Select(pp => new ProdutoResponse
                        {
                            Id = pp.Produto.Id,
                            Nome = pp.Produto.Nome,
                            Preco = pp.Produto.Preco
                        }).ToList()
                    })
                    .ToListAsync();
        }
    }
}