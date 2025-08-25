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
    public class ProdutoService : BaseService<Produto>, IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository)
            : base(produtoRepository)  // passa pro BaseService
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<Produto?> GetByNomeAsync(string email)
        {
            return await _produtoRepository.GetByNomeAsync(email);
        }

        public async Task NovoProdutoAsync(ProdutoRequest request)
        {
            //Produto(Nome) já cadastrado
            var res = await _produtoRepository.GetByNomeAsync(request.Nome);
            if (res != null)
                throw new ValidationException($"Produto {request.Nome} já cadastrado no sistema.");
            //Validadores
            //Nome
            if (request.Nome == string.Empty)
                throw new ValidationException($"Nome não informado");
            //Descricao
            if (request.Descricao == string.Empty)
                throw new ValidationException($"Descrição não informada");
            //Preço
            if (request.Preco <= 0)
                throw new ValidationException($"Preço não informado ou invalido");
            //Quantidade
            if (request.Quantidade <= 0)
                throw new ValidationException($"Quantidade não informada ou invalida");
    
            var obj = new Produto
            {
                Id = Guid.NewGuid(),
                Nome = request.Nome,
                Descricao = request.Descricao,
                Preco = request.Preco,
                Quantidade = request.Quantidade,
                CreatedAt = DateTime.Now
            };

            await _produtoRepository.CreateAsync(obj);
        }
    }
}