using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using static ControleEstoque.Application.Middlewares.ErrorHandlingMiddleware;

namespace ControleEstoque.Application.Services
{
    public class ProdutoService : BaseService<Produto>, IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository
            )
            : base(produtoRepository)  // passa pro BaseService
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<Produto?> GetByNomeAsync(string email)
        {
            return await _produtoRepository.GetByNomeAsync(email);
        }

        public async Task NovoProdutoAsync(ProdutoCadRequest request)
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
            try
            {
                var obj = new Produto
                {
                    Id = Guid.NewGuid(),
                    Nome = request.Nome,
                    Descricao = request.Descricao,
                    Preco = request.Preco,
                    Quantidade = 1,
                    CreatedAt = DateTime.UtcNow
                };

                await _produtoRepository.CreateAsync(obj);
            }
            finally
            {
            }
        }

        public async Task AtualizarProdutoAsync(ProdutoRequest request)
        {
            //Produto(Nome) já cadastrado
            var res = await _produtoRepository.GetByIdAsync(request.Id);
            if (res == null)
                throw new ValidationException($"Codigo {request.Id} do Produto não existe no sistema.");
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

            try
            {
                res.Nome = request.Nome;
                res.Descricao = request.Descricao;
                res.Preco = request.Preco;
                res.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(res);
            }
            finally
            {
            }
        }

        public async Task AtualizarProdutoAdminAsync(ProdutoAdminRequest request)
        {
            //Produto(Nome) já cadastrado
            var res = await _produtoRepository.GetByIdAsync(request.Id);
            if (res == null)
                throw new ValidationException($"Codigo {request.Id} do Produto não existe no sistema.");
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

            try
            {
                res.Nome = request.Nome;
                res.Descricao = request.Descricao;
                res.Preco = request.Preco;
                res.Quantidade = request.Quantidade;
                res.UpdatedAt = DateTime.UtcNow;


                await _repository.UpdateAsync(res);
            }
            finally
            {
            }
        }
    }
}