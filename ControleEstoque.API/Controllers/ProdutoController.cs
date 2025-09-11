using ControleEstoque.Application.Services;
using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using Mapster;
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

        [Authorize]
        [HttpPost("Cadastrar")]
        public async Task<IActionResult> Create([FromForm] ProdutoCadRequest request)
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
        [Authorize]
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

        #region Desabilitar ENDPOINTS HERDADOS DO BASE  
        //[NonAction]
        //public override async Task<ActionResult<IEnumerable<Pedido>>> GetAll()
        //{
        //    throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        //}

        //[NonAction]
        //public override async Task<ActionResult<Pedido>> GetById(Guid id)
        //{
        //    throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        //}

        [NonAction]
        public override async Task<ActionResult<Produto>> Create(Produto entity)
        {
            throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        }

        [NonAction]
        public override async Task<IActionResult> Update(Guid id, Produto entity)
        {
            throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        }

        [NonAction]
        public override async Task<IActionResult> Delete(Guid id)
        {
            throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        }
        #endregion
    }
}