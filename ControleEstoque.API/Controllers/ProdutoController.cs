using ControleEstoque.Application.Services;
using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleEstoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : BaseController<Produto>
    {
        private readonly IProdutoService _produtoService;
        public ProdutoController(IBaseRepository<Produto> repository, IProdutoService produtoService)
            : base(repository)
        {
            _produtoService = produtoService;
        }
        
        [HttpPost("Cadastrar")]
        public async Task<IActionResult> Create([FromForm] ProdutoRequest request)
        {
            try
            {
                await _produtoService.NovoProdutoAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Não foi possível cadastrar o usuário.\n{ex.Message}");
            }
        }

        [HttpPut("AtualizarProdutoEstoque")]
        public async Task<IActionResult> AtualizarProdutoEstoque([FromForm] ProdutoRequest request)
        {
            try
            {
                await _produtoService.AtualizarProdutoAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Não foi possível atualizar o produto.\n{ex.Message}");
            }
        }
        [Authorize(Policy = "ADMIN")]
        [HttpPut("AtualizarProdutoAdminEstoque")]
        public async Task<IActionResult> AtualizarProdutoAdminEstoque([FromForm] ProdutoAdminRequest request)
        {
            try
            {
                await _produtoService.AtualizarProdutoAdminAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Não foi possível atualizar o produto.\n{ex.Message}");
            }
        }

        //Desabilita o endpoint PUT padrão herdado do BaseController
        [NonAction]
        public override Task<IActionResult> Update(Guid id, Produto entity)
        {
            return base.Update(id, entity); // ou pode até deixar vazio
        }

    }
}