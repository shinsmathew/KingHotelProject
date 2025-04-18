﻿using AutoMapper;
using FluentValidation;
using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
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
        private readonly IValidator<HotelUpdateDto> _validator;
        private readonly ICacheService _cacheService;

        public UpdateHotelCommandHandler(
            IHotelRepository hotelRepository,
            IMapper mapper,
            IValidator<HotelUpdateDto> validator,
            ICacheService cacheService)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _validator = validator;
            _cacheService = cacheService;
        }

        public async Task Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.HotelUpdateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var hotel = await _hotelRepository.GetHotelByIdAsync(request.Id);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.Id);
            }

            _mapper.Map(request.HotelUpdateDto, hotel);
            await _hotelRepository.UpdateHotelAsync(hotel);

            // Invalidate caches
            await _cacheService.RemoveRedisCacheAsync("AllHotels");
            await _cacheService.RemoveRedisCacheAsync($"Hotel_{request.Id}");
            await _cacheService.RemoveRedisCacheAsync($"DishesByHotel_{request.Id}");
        }
    }
}