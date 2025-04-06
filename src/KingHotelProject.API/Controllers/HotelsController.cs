using FluentValidation;
using FluentValidation.AspNetCore;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Application.Features.Hotels.Commands;
using KingHotelProject.Application.Features.Hotels.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KingHotelProject.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<HotelCreateDto> _createValidator;
        private readonly IValidator<HotelUpdateDto> _updateValidator;

        public HotelsController(
            IMediator mediator,
            IValidator<HotelCreateDto> createValidator,
            IValidator<HotelUpdateDto> updateValidator)
        {
            _mediator = mediator;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HotelResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HotelResponseDto>>> GetAll()
        {
            var query = new GetAllHotelsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HotelResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelResponseDto>> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var query = new GetHotelByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(HotelResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HotelResponseDto>> Create(HotelCreateDto hotelCreateDto)
        {
            var validationResult = await _createValidator.ValidateAsync(hotelCreateDto);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }

            var command = new CreateHotelCommand { HotelCreateDto = hotelCreateDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.HotelId }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, HotelUpdateDto hotelUpdateDto)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var validationResult = await _updateValidator.ValidateAsync(hotelUpdateDto);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }

            var command = new UpdateHotelCommand { Id = id, HotelUpdateDto = hotelUpdateDto };
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

            var command = new DeleteHotelCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}

