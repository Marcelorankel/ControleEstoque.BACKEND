using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Enums;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using ControleEstoque.Core.Utils;
using Elastic.Apm;
using OpenTelemetry.Trace;
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
        private readonly Tracer _tracer;

        public ProdutoService(IProdutoRepository produtoRepository, TracerProvider tracerProvider)
            : base(produtoRepository)  // passa pro BaseService
        {
            _produtoRepository = produtoRepository;
            _tracer = tracerProvider.GetTracer("ProdutoService");
        }

        public async Task<Produto?> GetByNomeAsync(string email)
        {
            return await _produtoRepository.GetByNomeAsync(email);
        }

        public async Task NovoProdutoAsync(ProdutoRequest request)
        {
            //Trace
            using var span = _tracer.StartActiveSpan("NovoProduto");

            //Elastic APM
            var transaction = Agent.Tracer.CurrentTransaction;
            var elasticSpan = transaction?.StartSpan("NovoProduto", "custom");

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
                span.SetAttribute("produto.criado", request?.ToString() ?? "0");
                elasticSpan?.SetLabel("produto.criado", request?.ToString() ?? "0");
            }
            finally
            {
                elasticSpan?.End();
            }
        }

        public async Task AtualizarProdutoAsync(ProdutoRequest request)
        {
            //Trace
            using var span = _tracer.StartActiveSpan("AtualizarProdutoAsync");

            //Elastic APM
            var transaction = Agent.Tracer.CurrentTransaction;
            var elasticSpan = transaction?.StartSpan("AtualizarProdutoAsync", "custom");

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

                span.SetAttribute("produto.atualizado", request?.ToString() ?? "0");
                elasticSpan?.SetLabel("produto.atualizado", request?.ToString() ?? "0");
            }
            finally
            {
                elasticSpan?.End();
            }
        }

        public async Task AtualizarProdutoAdminAsync(ProdutoAdminRequest request)
        {
            //Trace
            using var span = _tracer.StartActiveSpan("AtualizarProdutoAdminAsync");

            //Elastic APM
            var transaction = Agent.Tracer.CurrentTransaction;
            var elasticSpan = transaction?.StartSpan("AtualizarProdutoAdminAsync", "custom");

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

                span.SetAttribute("produto.atualizadoAdmin", request?.ToString() ?? "0");
                elasticSpan?.SetLabel("produto.atualizadoAdmin", request?.ToString() ?? "0");
            }
            finally
            {
                elasticSpan?.End();
            }
        }
    }
}