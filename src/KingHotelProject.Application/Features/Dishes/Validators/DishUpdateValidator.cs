﻿using FluentValidation;
using KingHotelProject.Application.DTOs;

namespace KingHotelProject.Application.Features.Dishes.Validators
{
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