using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Enums;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using ControleEstoque.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ControleEstoque.Application.Middlewares.ErrorHandlingMiddleware;

namespace ControleEstoque.Application.Services
{
    public class PedidoService : BaseService<Pedido>, IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProdutoRepository _produtoRepository;

        public PedidoService(IPedidoRepository pedidoRepository, IUsuarioRepository usuarioRepository, IProdutoRepository produtoRepository)
            : base(pedidoRepository)  // passa pro BaseService
        {
            _pedidoRepository = pedidoRepository;
            _usuarioRepository = usuarioRepository;
            _produtoRepository = produtoRepository;
        }

        public async Task NovoPedidoAsync(PedidoRequest request)
        {
            //Validadores
            //DataPedido
            if (request.DataPedido == DateTime.MinValue)
                throw new ValidationException($"Data Pedido invalida");
            //DocumentoCliente
            if (request.DocumentoCliente == string.Empty)
                throw new ValidationException($"Documento cliente não informado");
            //IdUsuario
            if (request.IdUsuario == Guid.Empty)
                throw new ValidationException($"Id Usuario não informado");
            //ValorTotal
            if (request.ValorTotal <= 0)
                throw new ValidationException($"Valor total pedido incorreto ou não informado");
            //Valida Lista de Produtos
            foreach (var item in request.Produtos)
            {
                var aux = await _produtoRepository.GetByIdAsync(item.Id);
                if (aux == null)
                    throw new ValidationException($"Produto ID - {item.Id} não foi encontrado");
            }

            ////Quantidade
            //if (request.Quantidade <= 0)
            //    throw new ValidationException($"Quantidade não informada ou invalida");

            //Busca usuario no banco por ID
            var res = await _usuarioRepository.GetByIdAsync(request.IdUsuario);
            if (res == null)
                throw new ValidationException($"Usuario ID - {request.IdUsuario} não foi encontrado");

            var obj = new Pedido
            {
                Id = Guid.NewGuid(),
                IdUsuario = request.IdUsuario,
                DocumentoCliente = request.DocumentoCliente,
                DataPedido = request.DataPedido,
                ValorTotal = request.ValorTotal,
                CreatedAt = DateTime.UtcNow,
                Usuario = res
            };          

            await _pedidoRepository.CreateAsync(obj);

            //Rduz estoque dos produtos do pedido
            foreach (var item in request.Produtos)
            {
                var aux = await _produtoRepository.GetByIdAsync(item.Id);         
                aux.Quantidade -= item.Quantidade;

               await _produtoRepository.UpdateAsync(aux);

            }
        }
    }
}