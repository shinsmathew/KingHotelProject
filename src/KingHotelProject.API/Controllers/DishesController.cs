
using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.Features.Dishes.Commands;
using KingHotelProject.Application.Features.Dishes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KingHotelProject.API.Controllers
{

    /// <summary>
    /// Manages dish-related operations in the system
    /// </summary>
    /// <remarks>
    /// Provides CRUD operations for dishes with admin-only access for write operations
    /// </remarks>

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

        #region Read All Dishes

        /// <summary>
        /// Retrieves all dishes in the system
        /// </summary>
        /// <returns>List of all dishes</returns>
        /// <response code="200">Returns the list of dishes</response>

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DishResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DishResponseDto>>> GetAllDishes()
        {
            var query = new GetAllDishesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        #endregion

        #region Read Dish with DishID

        /// <summary>
        /// Retrieves a specific dish by ID
        /// </summary>
        /// <param name="id">The dish ID</param>
        /// <returns>The requested dish details</returns>
        /// <response code="200">Returns the requested dish</response>
        /// <response code="400">If the ID format is invalid</response>
        /// <response code="404">If the dish is not found</response>


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

        #endregion

        #region Create Dish

        /// <summary>
        /// Creates multiple dishes in bulk (Admin only)
        /// </summary>
        /// <param name="dishesBulkCreateDto">List of dishes to create</param>
        /// <returns>The created dishes</returns>
        /// <response code="201">Returns the created dishes</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not an admin</response>

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<DishResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DishResponseDto>>> CreateDishesBulk([FromBody] DishesBulkCreateDto dishesBulkCreateDto)
        {
            var command = new CreateDishesBulkCommand { DishesBulkCreateDto = dishesBulkCreateDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAllDishes), result);
        }
        #endregion


        #region Upadte Dish
        /// <summary>
        /// Updates an existing dish (Admin only)
        /// </summary>
        /// <param name="id">The dish ID to update</param>
        /// <param name="dishUpdateDto">Updated dish data</param>
        /// <returns>No content</returns>
        /// <response code="204">If update was successful</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not an admin</response>
        /// <response code="404">If the dish is not found</response>


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDishes(Guid id, [FromBody] DishUpdateDto dishUpdateDto)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid ID format");
            }

            var command = new UpdateDishCommand { Id = id, DishUpdateDto = dishUpdateDto };
            await _mediator.Send(command);
            return NoContent();
        }



        #endregion

        #region Delete Dish

        /// <summary>
        /// Deletes a dish (Admin only)
        /// </summary>
        /// <param name="id">The dish ID to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">If deletion was successful</response>
        /// <response code="400">If the ID format is invalid</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="403">If user is not an admin</response>
        /// <response code="404">If the dish is not found</response>

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

        #endregion 

    }
}