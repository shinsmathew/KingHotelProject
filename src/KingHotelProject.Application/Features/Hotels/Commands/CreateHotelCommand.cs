using AutoMapper;
using FluentValidation;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Hotels.Commands
{
    public class CreateHotelCommand : IRequest<HotelResponseDto>
    {
        public HotelCreateDto HotelCreateDto { get; set; }
    }

    public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, HotelResponseDto>
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<HotelCreateDto> _validator;
        private readonly ICacheService _cacheService;

        public CreateHotelCommandHandler(
            IHotelRepository hotelRepository,
            IMapper mapper,
            IValidator<HotelCreateDto> validator,
            ICacheService cacheService)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _validator = validator;
            _cacheService = cacheService;
        }

        public async Task<HotelResponseDto> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.HotelCreateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var hotel = _mapper.Map<Hotel>(request.HotelCreateDto);
            var createdHotel = await _hotelRepository.AddAsync(hotel);

            // Invalidate the AllHotels cache
            await _cacheService.RemoveAsync("AllHotels");

            return _mapper.Map<HotelResponseDto>(createdHotel);
        }
    }
}