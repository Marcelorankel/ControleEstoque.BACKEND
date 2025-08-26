using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace ControleEstoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController<T> : ControllerBase where T : class
    {
        private readonly IBaseRepository<T> _repository;

        public BaseController(IBaseRepository<T> repository)
        {
            _repository = repository;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<T>>> GetAll()
        {
            var entities = await _repository.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("GetBy{id}")]
        public async Task<ActionResult<T>> GetById(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public virtual async Task<ActionResult<T>> Create(T entity)
        {
            await _repository.CreateAsync(entity);
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(Guid id, T entity)
        {
            await _repository.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _repository.DeleteAsync(entity);
            return NoContent();
        }
    }
}