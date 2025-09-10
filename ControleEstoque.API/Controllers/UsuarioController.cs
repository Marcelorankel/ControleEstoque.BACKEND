using ControleEstoque.Application.Services;
using ControleEstoque.Core.Entities;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Models;
using Elastic.Apm.Api;
using Mapster;
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

        [Authorize]
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(Guid id, UsuarioEditRequest request)
        {
            var objUsuario = await _usuarioService.GetByIdAsync(id);

            if (objUsuario == null) {
                return NotFound();
            }

            request.Adapt(objUsuario);
            objUsuario.UpdatedAt = DateTime.UtcNow;

            await _usuarioService.UpdateAsync(objUsuario);

            return NoContent();
        }

        #region Desabilitar ENDPOINTS HERDADOS DO BASE  
        [NonAction]
        public override async Task<IActionResult> Update(Guid id, Usuario entity)
        {
            throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        }
        [NonAction]
        public override async Task<ActionResult<Usuario>> Create(Usuario entity)
        {
            throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        }
        #endregion
    }
}