
using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.Features.Hotels.Commands;
using KingHotelProject.Application.Features.Hotels.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KingHotelProject.API.Controllers
{
    /// <summary>
    /// Manages hotel-related operations in the system
    /// </summary>
    /// <remarks>
    /// Provides CRUD operations for hotels with admin-only access for write operations
    /// </remarks>

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HotelsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Read Hotels
        /// <summary>
        /// Retrieves all hotels in the system
        /// </summary>
        /// <returns>List of all hotels</returns>
        /// <response code="200">Returns the list of hotels</response>


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HotelResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HotelResponseDto>>> GetAllHotelData()
        {
            var query = new GetAllHotelsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        #endregion

        #region Read Hotel with HotelID

        /// <summary>
        /// Retrieves a specific hotel by ID
        /// </summary>
        /// <param name="id">The hotel ID</param>
        /// <returns>The requested hotel details</returns>
        /// <response code="200">Returns the requested hotel</response>
        /// <response code="400">If the ID format is invalid</response>
        /// <response code="404">If the hotel is not found</response>


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HotelResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelResponseDto>> GetHotalDataById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var query = new GetHotelByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        #endregion


        #region Create Hotel

        /// <summary>
        /// Creates multiple hotels in bulk (Admin only)
        /// </summary>
        /// <param name="hotelsBulkCreateDto">List of hotels to create</param>
        /// <returns>The created hotels</returns>
        /// <response code="201">Returns the created hotels</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not an admin</response>

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<HotelResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<HotelResponseDto>>> CreateHotelsBulk([FromBody] HotelsBulkCreateDto hotelsBulkCreateDto)
        {
            var command = new CreateHotelsBulkCommand { HotelsBulkCreateDto = hotelsBulkCreateDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAllHotelData), result);
        }

        #endregion


        #region Upadte Hotel
        /// <summary>
        /// Updates an existing hotel (Admin only)
        /// </summary>
        /// <param name="id">The hotel ID to update</param>
        /// <param name="hotelUpdateDto">Updated hotel data</param>
        /// <returns>No content</returns>
        /// <response code="204">If update was successful</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not an admin</response>
        /// <response code="404">If the hotel is not found</response>

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] HotelUpdateDto hotelUpdateDto)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var command = new UpdateHotelCommand { Id = id, HotelUpdateDto = hotelUpdateDto };
            await _mediator.Send(command);
            return NoContent();
        }

        #endregion

        #region Delete Hotel
        /// <summary>
        /// Deletes a hotel (Admin only)
        /// </summary>
        /// <param name="id">The hotel ID to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">If deletion was successful</response>
        /// <response code="400">If the ID format is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not an admin</response>
        /// <response code="404">If the hotel is not found</response>


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var command = new DeleteHotelCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

        #endregion
    }
}