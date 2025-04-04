
using KingHotelProject.Application.DTOs;
using KingHotelProject.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KingHotelProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(UserRegisterDto userRegisterDto)
        {
            var command = new RegisterUserCommand { UserRegisterDto = userRegisterDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto userLoginDto)
        {
            var command = new LoginUserCommand { UserLoginDto = userLoginDto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}

