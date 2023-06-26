using API.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

    public class GeneralController<TRepository, TEntity> : ControllerBase
    where TRepository : IGeneralRepository<TEntity>
    where TEntity : class
    {
        protected readonly TRepository _repository;
        public GeneralController(TRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var entity = _repository.GetAll();
            if (!entity.Any())
            {
                return NotFound();
            }
            return Ok(entity);
        }

        [HttpGet("{id}")]
        public IActionResult GetByGuid(Guid id)
        {
            var entity = _repository.GetByGuid(id);
            if (entity is null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        [HttpPost]
        public IActionResult Create(TEntity entity)
        {
            var createdEntity = _repository.Create(entity);
            return Ok(createdEntity);
        }

        [HttpPut]
        public IActionResult Update(TEntity entity)
        {
            var isUpdated = _repository.Update(entity);
            if (!isUpdated)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete ("{id}")]
        public IActionResult Delete(Guid id)
        {
            var isDeleted = _repository.Delete(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return Ok();
        }
    }

