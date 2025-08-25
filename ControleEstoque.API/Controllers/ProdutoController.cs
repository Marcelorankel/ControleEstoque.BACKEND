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
        //[Authorize(Policy = "ADMIN")]
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

    }
}