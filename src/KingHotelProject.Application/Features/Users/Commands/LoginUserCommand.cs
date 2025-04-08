
using KingHotelProject.Core.Interfaces;
using MediatR;
using FluentValidation;
using KingHotelProject.Application.DTOs.Users;

namespace KingHotelProject.Application.Features.Users.Commands
{
    public class LoginUserCommand : IRequest<AuthResponseDto>
    {
        public UserLoginDto UserLoginDto { get; set; }
    }

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly IValidator<UserLoginDto> _validator;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            IIdentityService identityService,
            IValidator<UserLoginDto> validator)
        {
            _userRepository = userRepository;
            _identityService = identityService;
            _validator = validator;
        }

        public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.UserLoginDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var user = await _userRepository.GetByUserNameAsync(request.UserLoginDto.UserName);
            if (user == null)
            {
                throw new Core.Exceptions.UnauthorizedAccessException("Invalid credentials");
            }

            if (!_identityService.VerifyPassword(user.PasswordHash, request.UserLoginDto.Password))
            {
                throw new Core.Exceptions.UnauthorizedAccessException("Invalid credentials");
            }

            var token = _identityService.GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserResponseDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = user.Role,
                    CreatedDate = user.CreatedDate
                }
            };
        }
    }
}