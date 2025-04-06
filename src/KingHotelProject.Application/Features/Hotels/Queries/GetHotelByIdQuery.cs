using AutoMapper;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Hotels.Queries
{
    public class GetHotelByIdQuery : IRequest<HotelResponseDto>
    {
        public Guid Id { get; set; }
    }

    public class GetHotelByIdQueryHandler : IRequestHandler<GetHotelByIdQuery, HotelResponseDto>
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public GetHotelByIdQueryHandler(
            IHotelRepository hotelRepository,
            IMapper mapper,
            ICacheService cacheService)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<HotelResponseDto> Handle(GetHotelByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Hotel_{request.Id}";

            // Check cache first
            var cachedHotel = await _cacheService.GetRedisCacheAsync<HotelResponseDto>(cacheKey);
            if (cachedHotel != null)
            {
                return cachedHotel;
            }

            // If not in cache, get from database
            var hotel = await _hotelRepository.GetHotelByIdAsync(request.Id);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.Id);
            }

            var result = _mapper.Map<HotelResponseDto>(hotel);

            // Cache the result
            await _cacheService.SetRedisCacheAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}