using AutoMapper;
using FluentValidation;
using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Hotels.Commands
{
    public class CreateHotelsBulkCommand : IRequest<IEnumerable<HotelResponseDto>>
    {
        public HotelsBulkCreateDto HotelsBulkCreateDto { get; set; }
    }

    public class CreateHotelsBulkCommandHandler : IRequestHandler<CreateHotelsBulkCommand, IEnumerable<HotelResponseDto>>
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IValidator<HotelsBulkCreateDto> _validator;

        public CreateHotelsBulkCommandHandler(
            IHotelRepository hotelRepository,
            IMapper mapper,
            ICacheService cacheService,
            IValidator<HotelsBulkCreateDto> validator)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _validator = validator;
        }

        public async Task<IEnumerable<HotelResponseDto>> Handle(CreateHotelsBulkCommand request, CancellationToken cancellationToken)
        {
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request.HotelsBulkCreateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Create all hotels
            var hotels = _mapper.Map<List<Hotel>>(request.HotelsBulkCreateDto.Hotels);
            var createdHotels = new List<Hotel>();

            foreach (var hotel in hotels)
            {
                createdHotels.Add(await _hotelRepository.AddHotelAsync(hotel));
            }

            // Invalidate the cache
            await _cacheService.RemoveRedisCacheAsync("AllHotels");

            return _mapper.Map<IEnumerable<HotelResponseDto>>(createdHotels);
        }
    }
}