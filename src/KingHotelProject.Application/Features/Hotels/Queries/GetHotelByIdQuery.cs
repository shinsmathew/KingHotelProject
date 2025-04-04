
using AutoMapper;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Repositories;
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

        public GetHotelByIdQueryHandler(IHotelRepository hotelRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        public async Task<HotelResponseDto> Handle(GetHotelByIdQuery request, CancellationToken cancellationToken)
        {
            var hotel = await _hotelRepository.GetByIdAsync(request.Id);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.Id);
            }

            return _mapper.Map<HotelResponseDto>(hotel);
        }
    }
}