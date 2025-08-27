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
    public class PedidoController : BaseController<Pedido>
    {
        private readonly IPedidoService _pedidoService;
        public PedidoController(IBaseRepository<Pedido> repository, IPedidoService pedidoService)
            : base(repository)
        {
            _pedidoService = pedidoService;
        }

        [Authorize]
        [HttpPost("NovoPedido")]
        public async Task<ActionResult<Pedido>> NovoPedido(PedidoRequest request)
        {
            try
            {
                await _pedidoService.NovoPedidoFilaAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Não foi possível efetuar o pedido.\n{ex.Message}");
            }
        }
        [Authorize]
        [HttpGet("GetPedidoPorId")]
        public async Task<IActionResult> GetPedidoPorId(Guid id)
        {
            try
            {
                var res = await _pedidoService.ObterPedidoPorIdAsync(id);

                if (res == null)
                    return NotFound($"Nenhum pedido encontrado para este id - {id}.");

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest($"Não foi buscar pedidos.\n{ex.Message}");
            }
        }
        [Authorize]
        [HttpGet("GetTodosPedidos")]
        public async Task<IActionResult> GetTodosPedidos()
        {
            try
            {
                var res = await _pedidoService.ObterTodosPedidosAsync();

                if (res == null || res.Count == 0)
                    return NotFound("Nenhum pedido encontrado.");

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest($"Não foi buscar pedidos.\n{ex.Message}");
            }
        }
        [Authorize]
        [HttpGet("GetPedidosPorEmailUsuario")]
        public async Task<IActionResult> GetPedidosPorEmailUsuario(string email)
        {
            try
            {
                var res = await _pedidoService.ObterPedidosPorEmailUsuarioAsync(email);

                if (res == null || res.Count == 0)
                    return NotFound($"Nenhum pedido encontrado para este email de usuário {email}.");

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest($"Não foi buscar pedidos.\n{ex.Message}");
            }
        }

        #region Desabilitar ENDPOINTS HERDADOS DO BASE  
        [NonAction]
        public override async Task<ActionResult<IEnumerable<Pedido>>> GetAll()
        {
            throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        }

        [NonAction]
        public override async Task<ActionResult<Pedido>> GetById(Guid id)
        {
            throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        }

        [NonAction]
        public override async Task<ActionResult<Pedido>> Create(Pedido entity)
        {
            throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        }

        [NonAction]
        public override async Task<IActionResult> Update(Guid id, Pedido entity)
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