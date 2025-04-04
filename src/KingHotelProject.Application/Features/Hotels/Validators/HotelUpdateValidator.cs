// KingHotelProject.Application/Features/Hotels/Validators/HotelUpdateValidator.cs
using FluentValidation;
using KingHotelProject.Application.DTOs;

namespace KingHotelProject.Application.Features.Hotels.Validators
{
    public class HotelUpdateValidator : AbstractValidator<HotelUpdateDto>
    {
        public HotelUpdateValidator()
        {
            RuleFor(x => x.HotelName)
                .NotEmpty().WithMessage("Hotel name is required")
                .MaximumLength(100).WithMessage("Hotel name must not exceed 100 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(100).WithMessage("Address must not exceed 100 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(50).WithMessage("City must not exceed 50 characters");

            RuleFor(x => x.Zip)
                .NotEmpty().WithMessage("Zip code is required")
                .MaximumLength(50).WithMessage("Zip code must not exceed 50 characters")
                .Matches(@"^\d+$").WithMessage("Zip code must contain only numbers");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(50).WithMessage("Country must not exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email is required");

            RuleFor(x => x.PhoneNumber1)
                .NotEmpty().WithMessage("Primary phone number is required")
                .Matches(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$")
                .WithMessage("A valid phone number is required");

            RuleFor(x => x.PhoneNumber2)
                .Matches(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber2))
                .WithMessage("A valid phone number is required");
        }
    }
}