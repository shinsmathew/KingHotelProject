// KingHotelProject.Application/Features/Dishes/Queries/GetAllDishesQuery.cs
using AutoMapper;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Dishes.Queries
{
    public class GetAllDishesQuery : IRequest<IEnumerable<DishResponseDto>>
    {
    }

    public class GetAllDishesQueryHandler : IRequestHandler<GetAllDishesQuery, IEnumerable<DishResponseDto>>
    {
        private readonly IDishRepository _dishRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CACHE_KEY = "AllDishes";

        public GetAllDishesQueryHandler(
            IDishRepository dishRepository,
            IMapper mapper,
            ICacheService cacheService)
        {
            _dishRepository = dishRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<DishResponseDto>> Handle(GetAllDishesQuery request, CancellationToken cancellationToken)
        {
            // Check cache first
            var cachedDishes = await _cacheService.GetAsync<IEnumerable<DishResponseDto>>(CACHE_KEY);
            if (cachedDishes != null)
            {
                return cachedDishes;
            }

            // If not in cache, get from database
            var dishes = await _dishRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<DishResponseDto>>(dishes);

            // Cache the result
            await _cacheService.SetAsync(CACHE_KEY, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}