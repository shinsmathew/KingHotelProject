
using KingHotelProject.Application.DTOs.Users;
using KingHotelProject.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KingHotelProject.API.Controllers
{
    /// <summary>
    /// Handles user authentication operations including registration and login
    /// </summary>
    /// <remarks>
    /// Provides endpoints for user authentication and authorization
    /// </remarks>

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region User Registration

        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="userRegisterDto">User registration data</param>
        /// <returns>Authentication response with JWT token and user details</returns>
        /// <response code="200">Returns the auth token and user details</response>
        /// <response code="400">If the request data is invalid</response>

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var command = new RegisterUserCommand { UserRegisterDto = userRegisterDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        #endregion

        #region  User login

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="userLoginDto">User login credentials</param>
        /// <returns>Authentication response with JWT token and user details</returns>
        /// <response code="200">Returns the auth token and user details</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If authentication fails</response>


        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginDto userLoginDto)
        {
            var command = new LoginUserCommand { UserLoginDto = userLoginDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        #endregion
    }
}