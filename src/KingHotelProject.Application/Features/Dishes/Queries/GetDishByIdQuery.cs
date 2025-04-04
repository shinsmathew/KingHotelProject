
using AutoMapper;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Dishes.Queries
{
    public class GetDishByIdQuery : IRequest<DishResponseDto>
    {
        public Guid Id { get; set; }
    }

    public class GetDishByIdQueryHandler : IRequestHandler<GetDishByIdQuery, DishResponseDto>
    {
        private readonly IDishRepository _dishRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public GetDishByIdQueryHandler(
            IDishRepository dishRepository,
            IMapper mapper,
            ICacheService cacheService)
        {
            _dishRepository = dishRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<DishResponseDto> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Dish_{request.Id}";

            // Check cache first
            var cachedDish = await _cacheService.GetAsync<DishResponseDto>(cacheKey);
            if (cachedDish != null)
            {
                return cachedDish;
            }

            // If not in cache, get from database
            var dish = await _dishRepository.GetByIdAsync(request.Id);
            if (dish == null)
            {
                throw new NotFoundException(nameof(Dish), request.Id);
            }

            var result = _mapper.Map<DishResponseDto>(dish);

            // Cache the result
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}