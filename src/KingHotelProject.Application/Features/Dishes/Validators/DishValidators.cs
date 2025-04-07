using FluentValidation;
using KingHotelProject.Application.DTOs;

namespace KingHotelProject.Application.Features.Dishes.Validators
{
    public class DishCreateValidator : AbstractValidator<DishCreateDto>
    {
        public DishCreateValidator()
        {
            RuleFor(x => x.DishName)
                .NotEmpty().WithMessage("Dish name is required")
                .MaximumLength(50).WithMessage("Dish name must not exceed 50 characters");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required")
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.HotelId)
                .NotEmpty().WithMessage("Hotel ID is required");
        }
    }

    public class DishesBulkCreateValidator : AbstractValidator<DishesBulkCreateDto>
    {
        public DishesBulkCreateValidator()
        {
            RuleFor(x => x.Dishes)
                .NotEmpty().WithMessage("At least one dish is required")
                .Must(d => d.Count <= 100).WithMessage("Cannot create more than 100 dishes at once");

            RuleForEach(x => x.Dishes).SetValidator(new DishCreateValidator());
        }
    }

    public class DishUpdateValidator : AbstractValidator<DishUpdateDto>
    {
        public DishUpdateValidator()
        {
            RuleFor(x => x.DishName)
                .NotEmpty().WithMessage("Dish name is required")
                .MaximumLength(50).WithMessage("Dish name must not exceed 50 characters");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required")
                .GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
}