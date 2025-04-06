using FluentValidation;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Enums;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
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
        private readonly IValidator<UserRegisterDto> _validator;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IIdentityService identityService,
            IValidator<UserRegisterDto> validator)
        {
            _userRepository = userRepository;
            _identityService = identityService;
            _validator = validator;
        }

        public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.UserRegisterDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (!Enum.IsDefined(typeof(UserRole), request.UserRegisterDto.Role))
            {
                throw new BadRequestException($"Invalid role value: {request.UserRegisterDto.Role}. Valid roles are: " +
                                             string.Join(", ", Enum.GetNames(typeof(UserRole))
                                                             .Select((name, index) => $"{name} ({index})")));
            }

            // Check username uniqueness
            var existingUserByName = await _userRepository.GetByUserNameAsync(request.UserRegisterDto.UserName);
            if (existingUserByName != null)
            {
                throw new BadRequestException("Username already exists");
            }

            // Check email uniqueness
            var existingUserByEmail = await _userRepository.GetByEmailAsync(request.UserRegisterDto.Email);
            if (existingUserByEmail != null)
            {
                throw new BadRequestException("Email already registered");
            }

            var user = new User
            {
                FirstName = request.UserRegisterDto.FirstName,
                LastName = request.UserRegisterDto.LastName,
                Email = request.UserRegisterDto.Email,
                UserName = request.UserRegisterDto.UserName,
                PasswordHash = _identityService.HashPassword(request.UserRegisterDto.Password),
                Role = (UserRole)request.UserRegisterDto.Role
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