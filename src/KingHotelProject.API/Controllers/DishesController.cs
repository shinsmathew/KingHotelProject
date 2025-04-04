// KingHotelProject.API/Controllers/DishesController.cs
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

        public DishesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DishResponseDto>>> GetAll()
        {
            var query = new GetAllDishesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DishResponseDto>> GetById(Guid id)
        {
            var query = new GetDishByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<DishResponseDto>> Create(DishCreateDto dishCreateDto)
        {
            var command = new CreateDishCommand { DishCreateDto = dishCreateDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.DishId }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, DishUpdateDto dishUpdateDto)
        {
            var command = new UpdateDishCommand { Id = id, DishUpdateDto = dishUpdateDto };
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteDishCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("by-hotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<DishResponseDto>>> GetByHotelId(Guid hotelId)
        {
            var query = new GetDishesByHotelIdQuery { HotelId = hotelId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}