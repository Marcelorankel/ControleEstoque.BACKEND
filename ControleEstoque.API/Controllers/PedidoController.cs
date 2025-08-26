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
        //[Authorize(Policy = "ADMIN")]
        //[HttpPost("Cadastrar")]
        //public async Task<IActionResult> Create([FromForm] UsuarioRequest request)
        //{
        //    try
        //    {
        //        await _pedidoService.NovoUsuarioAsync(request);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Não foi possível cadastrar o usuário.\n{ex.Message}");
        //    }
        //}
        [HttpPost("NovoPedido")]
        public async Task<ActionResult<Pedido>> NovoPedido(PedidoRequest request)
        {
            {
                try
                {
                    await _pedidoService.NovoPedidoAsync(request);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Não foi possível efetuar o pedido.\n{ex.Message}");
                }
            }
        }


            [NonAction] // Desabilita o endpoint POST padrão herdado do BaseController
        public override async Task<ActionResult<Pedido>> Create(Pedido entity)
        {
            // não implementa nada ou pode até lançar exceção se chamado via código
            throw new NotImplementedException("Este endpoint não está disponível neste controller.");
        }

    }
}