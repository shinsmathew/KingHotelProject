using AutoMapper;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Repositories;
using MediatR;

namespace KingHotelProject.Application.Features.Hotels.Commands
{
    public class UpdateHotelCommand : IRequest
    {
        public Guid Id { get; set; }
        public HotelUpdateDto HotelUpdateDto { get; set; }
    }

    public class UpdateHotelCommandHandler : IRequestHandler<UpdateHotelCommand>
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;

        public UpdateHotelCommandHandler(IHotelRepository hotelRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        public async Task Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _hotelRepository.GetByIdAsync(request.Id);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.Id);
            }

            _mapper.Map(request.HotelUpdateDto, hotel);
            await _hotelRepository.UpdateAsync(hotel);
        }
    }
}