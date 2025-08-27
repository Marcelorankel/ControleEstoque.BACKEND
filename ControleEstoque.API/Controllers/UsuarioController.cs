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
    public class UsuarioController : BaseController<Usuario>
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IBaseRepository<Usuario> repository, IUsuarioService usuarioService)
            : base(repository)
        {
            _usuarioService = usuarioService;
        }
        [HttpPost("Cadastrar")]
        public async Task<IActionResult> Create([FromForm] UsuarioRequest request)
        {
            try
            {
                await _usuarioService.NovoUsuarioAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Não foi possível cadastrar o usuário.\n{ex.Message}");
            }
        }
    }
}