using AutoMapper;
using FluentValidation;
using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Dishes.Commands
{
    public class CreateDishesBulkCommand : IRequest<IEnumerable<DishResponseDto>>
    {
        public DishesBulkCreateDto DishesBulkCreateDto { get; set; }
    }

    public class CreateDishesBulkCommandHandler : IRequestHandler<CreateDishesBulkCommand, IEnumerable<DishResponseDto>>
    {
        private readonly IDishRepository _dishRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IValidator<DishesBulkCreateDto> _validator;

        public CreateDishesBulkCommandHandler(
            IDishRepository dishRepository,
            IHotelRepository hotelRepository,
            IMapper mapper,
            ICacheService cacheService,
            IValidator<DishesBulkCreateDto> validator)
        {
            _dishRepository = dishRepository;
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _validator = validator;
        }

        public async Task<IEnumerable<DishResponseDto>> Handle(CreateDishesBulkCommand request, CancellationToken cancellationToken)
        {
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request.DishesBulkCreateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Check all hotels exist in a single query
            var hotelIds = request.DishesBulkCreateDto.Dishes.Select(d => d.HotelId).Distinct().ToList();
            var hotels = await _hotelRepository.GetHotelByIdsAsync(hotelIds);

            var missingHotelIds = hotelIds.Except(hotels.Select(h => h.HotelId)).ToList();
            if (missingHotelIds.Any())
            {
                throw new NotFoundException(nameof(Hotel), string.Join(", ", missingHotelIds));
            }

            // Create all dishes
            var dishes = _mapper.Map<List<Dish>>(request.DishesBulkCreateDto.Dishes);
            var createdDishes = new List<Dish>();

            foreach (var dish in dishes)
            {
                createdDishes.Add(await _dishRepository.AddDishAsync(dish));
            }

            // Invalidate relevant caches
            await _cacheService.RemoveRedisCacheAsync("AllDishes");
            foreach (var hotelId in hotelIds)
            {
                await _cacheService.RemoveRedisCacheAsync($"DishesByHotel_{hotelId}");
            }

            return _mapper.Map<IEnumerable<DishResponseDto>>(createdDishes);
        }
    }
}