// KingHotelProject.Application/Features/Dishes/Queries/GetDishesByHotelIdQuery.cs
using AutoMapper;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Dishes.Queries
{
    public class GetDishesByHotelIdQuery : IRequest<IEnumerable<DishResponseDto>>
    {
        public Guid HotelId { get; set; }
    }

    public class GetDishesByHotelIdQueryHandler : IRequestHandler<GetDishesByHotelIdQuery, IEnumerable<DishResponseDto>>
    {
        private readonly IDishRepository _dishRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public GetDishesByHotelIdQueryHandler(
            IDishRepository dishRepository,
            IMapper mapper,
            ICacheService cacheService)
        {
            _dishRepository = dishRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<DishResponseDto>> Handle(GetDishesByHotelIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"DishesByHotel_{request.HotelId}";

            // Check cache first
            var cachedDishes = await _cacheService.GetAsync<IEnumerable<DishResponseDto>>(cacheKey);
            if (cachedDishes != null)
            {
                return cachedDishes;
            }

            // If not in cache, get from database
            var dishes = await _dishRepository.GetByHotelIdAsync(request.HotelId);
            var result = _mapper.Map<IEnumerable<DishResponseDto>>(dishes);

            // Cache the result
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}