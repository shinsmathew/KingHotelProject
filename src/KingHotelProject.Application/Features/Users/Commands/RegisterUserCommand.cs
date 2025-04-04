
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Identity;
using MediatR;

namespace KingHotelProject.Application.Features.Users.Commands
{
    public class RegisterUserCommand : IRequest<AuthResponseDto>
    {
        public UserRegisterDto UserRegisterDto { get; set; }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;

        public RegisterUserCommandHandler(IUserRepository userRepository, IIdentityService identityService)
        {
            _userRepository = userRepository;
            _identityService = identityService;
        }

        public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByUserNameAsync(request.UserRegisterDto.UserName);
            if (existingUser != null)
            {
                throw new BadRequestException("Username already exists");
            }

            var user = new User
            {
                FirstName = request.UserRegisterDto.FirstName,
                LastName = request.UserRegisterDto.LastName,
                Email = request.UserRegisterDto.Email,
                UserName = request.UserRegisterDto.UserName,
                PasswordHash = _identityService.HashPassword(request.UserRegisterDto.Password),
                Role = request.UserRegisterDto.Role
            };

            await _userRepository.AddAsync(user);

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

