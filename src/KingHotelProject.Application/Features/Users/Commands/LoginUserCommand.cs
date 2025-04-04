using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Identity;
using KingHotelProject.Infrastructure.Repositories;
using MediatR;

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

        public LoginUserCommandHandler(IUserRepository userRepository, IIdentityService identityService)
        {
            _userRepository = userRepository;
            _identityService = identityService;
        }

        public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUserNameAsync(request.UserLoginDto.UserName);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            if (!_identityService.VerifyPassword(user.PasswordHash, request.UserLoginDto.Password))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
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