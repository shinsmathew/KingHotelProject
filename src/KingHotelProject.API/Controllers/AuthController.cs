using FluentValidation;
using FluentValidation.AspNetCore;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<UserRegisterDto> _registerValidator;
    private readonly IValidator<UserLoginDto> _loginValidator;

    public AuthController(
        IMediator mediator,
        IValidator<UserRegisterDto> registerValidator,
        IValidator<UserLoginDto> loginValidator)
    {
        _mediator = mediator;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register(UserRegisterDto userRegisterDto)
    {
        var validationResult = await _registerValidator.ValidateAsync(userRegisterDto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return ValidationProblem(ModelState);
        }

        var command = new RegisterUserCommand { UserRegisterDto = userRegisterDto };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto userLoginDto)
    {
        var validationResult = await _loginValidator.ValidateAsync(userLoginDto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return ValidationProblem(ModelState);
        }

        var command = new LoginUserCommand { UserLoginDto = userLoginDto };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}