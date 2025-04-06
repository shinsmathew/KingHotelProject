using AutoMapper;
using FluentValidation;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Dishes.Commands
{
    public class UpdateDishCommand : IRequest, INotification
    {
        public Guid Id { get; set; }
        public DishUpdateDto DishUpdateDto { get; set; }
    }

    public class UpdateDishCommandHandler : IRequestHandler<UpdateDishCommand>
    {
        private readonly IDishRepository _dishRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IValidator<DishUpdateDto> _validator;

        public UpdateDishCommandHandler(
            IDishRepository dishRepository,
            IMapper mapper,
            ICacheService cacheService,
            IValidator<DishUpdateDto> validator)
        {
            _dishRepository = dishRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _validator = validator;
        }

        public async Task Handle(UpdateDishCommand request, CancellationToken cancellationToken)
        {
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request.DishUpdateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Get the dish
            var dish = await _dishRepository.GetByIdAsync(request.Id);
            if (dish == null)
            {
                throw new NotFoundException(nameof(Dish), request.Id);
            }

            // Update the dish
            _mapper.Map(request.DishUpdateDto, dish);
            await _dishRepository.UpdateAsync(dish);

            // Invalidate relevant caches
            await _cacheService.RemoveAsync("AllDishes");
            await _cacheService.RemoveAsync($"DishesByHotel_{dish.HotelId}");
            await _cacheService.RemoveAsync($"Dish_{request.Id}");
        }
    }
}

