
using KingHotelProject.Application.DTOs.Dishes;
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

        public DishesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DishResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DishResponseDto>>> GetAllDishes()
        {
            var query = new GetAllDishesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DishResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishResponseDto>> GetDishesById(Guid id)
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
        [ProducesResponseType(typeof(IEnumerable<DishResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DishResponseDto>>> CreateDishesBulk(DishesBulkCreateDto dishesBulkCreateDto)
        {
            var command = new CreateDishesBulkCommand { DishesBulkCreateDto = dishesBulkCreateDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAllDishes), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDishes(Guid id, DishUpdateDto dishUpdateDto)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var command = new UpdateDishCommand { Id = id, DishUpdateDto = dishUpdateDto };
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDishes(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var command = new DeleteDishCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

       
       
    }
}