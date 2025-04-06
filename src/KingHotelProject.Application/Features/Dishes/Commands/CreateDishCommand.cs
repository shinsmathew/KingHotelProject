using AutoMapper;
using FluentValidation;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Dishes.Commands
{
    public class CreateDishCommand : IRequest<DishResponseDto>, INotification
    {
        public DishCreateDto DishCreateDto { get; set; }
    }

    public class CreateDishCommandHandler : IRequestHandler<CreateDishCommand, DishResponseDto>
    {
        private readonly IDishRepository _dishRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IValidator<DishCreateDto> _validator;

        public CreateDishCommandHandler(
            IDishRepository dishRepository,
            IHotelRepository hotelRepository,
            IMapper mapper,
            ICacheService cacheService,
            IValidator<DishCreateDto> validator)
        {
            _dishRepository = dishRepository;
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _validator = validator;
        }

        public async Task<DishResponseDto> Handle(CreateDishCommand request, CancellationToken cancellationToken)
        {
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request.DishCreateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Check if hotel exists
            var hotel = await _hotelRepository.GetByIdAsync(request.DishCreateDto.HotelId);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.DishCreateDto.HotelId);
            }

            // Create the dish
            var dish = _mapper.Map<Dish>(request.DishCreateDto);
            var createdDish = await _dishRepository.AddAsync(dish);

            // Invalidate relevant caches
            await _cacheService.RemoveAsync("AllDishes");
            await _cacheService.RemoveAsync($"DishesByHotel_{request.DishCreateDto.HotelId}");

            return _mapper.Map<DishResponseDto>(createdDish);
        }
    }
}

