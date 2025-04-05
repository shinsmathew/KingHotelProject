using FluentValidation;
using KingHotelProject.Application.DTOs;

namespace KingHotelProject.Application.Features.Users.Validators
{
    public class UserLoginValidator : AbstractValidator<UserLoginDto> 
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}