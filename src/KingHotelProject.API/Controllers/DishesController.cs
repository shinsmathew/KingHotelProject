using FluentValidation;
using FluentValidation.AspNetCore;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Application.Features.Dishes.Commands;
using KingHotelProject.Application.Features.Dishes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KingHotelProject.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<DishCreateDto> _createValidator;
        private readonly IValidator<DishUpdateDto> _updateValidator;

        public DishesController(IMediator mediator,IValidator<DishCreateDto> createValidator,IValidator<DishUpdateDto> updateValidator)
        {
            _mediator = mediator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DishResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DishResponseDto>>> GetAll()
        {
            var query = new GetAllDishesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DishResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishResponseDto>> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var query = new GetDishByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(DishResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DishResponseDto>> Create(DishCreateDto dishCreateDto)
        {
            var validationResult = await _createValidator.ValidateAsync(dishCreateDto);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }

            var command = new CreateDishCommand { DishCreateDto = dishCreateDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.DishId }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, DishUpdateDto dishUpdateDto)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var validationResult = await _updateValidator.ValidateAsync(dishUpdateDto);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }

            var command = new UpdateDishCommand { Id = id, DishUpdateDto = dishUpdateDto };
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var command = new DeleteDishCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("by-hotel/{hotelId}")]
        [ProducesResponseType(typeof(IEnumerable<DishResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DishResponseDto>>> GetByHotelId(Guid hotelId)
        {
            if (hotelId == Guid.Empty)
            {
                return BadRequest("Invalid hotel ID format");
            }

            var query = new GetDishesByHotelIdQuery { HotelId = hotelId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

