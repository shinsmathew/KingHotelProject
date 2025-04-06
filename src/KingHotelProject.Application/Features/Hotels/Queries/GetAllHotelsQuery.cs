using AutoMapper;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Hotels.Queries
{
    public class GetAllHotelsQuery : IRequest<IEnumerable<HotelResponseDto>>
    {
    }

    public class GetAllHotelsQueryHandler : IRequestHandler<GetAllHotelsQuery, IEnumerable<HotelResponseDto>>
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CACHE_KEY = "AllHotels";

        public GetAllHotelsQueryHandler(
            IHotelRepository hotelRepository,
            IMapper mapper,
            ICacheService cacheService)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<HotelResponseDto>> Handle(GetAllHotelsQuery request, CancellationToken cancellationToken)
        {
            // Check cache first
            var cachedHotels = await _cacheService.GetRedisCacheAsync<IEnumerable<HotelResponseDto>>(CACHE_KEY);
            if (cachedHotels != null)
            {
                return cachedHotels;
            }

            // If not in cache, get from database
            var hotels = await _hotelRepository.GetAllHotelAsync();
            var result = _mapper.Map<IEnumerable<HotelResponseDto>>(hotels);

            // Cache the result
            await _cacheService.SetRedisCacheAsync(CACHE_KEY, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}