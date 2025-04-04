
using AutoMapper;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Repositories;
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

        public GetAllHotelsQueryHandler(IHotelRepository hotelRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<HotelResponseDto>> Handle(GetAllHotelsQuery request, CancellationToken cancellationToken)
        {
            var hotels = await _hotelRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<HotelResponseDto>>(hotels);
        }
    }
}