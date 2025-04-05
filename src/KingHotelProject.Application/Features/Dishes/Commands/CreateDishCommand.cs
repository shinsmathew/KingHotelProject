using AutoMapper;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Repositories;
using MediatR;

namespace KingHotelProject.Application.Features.Dishes.Commands
{
    public class CreateDishCommand : IRequest<DishResponseDto>, INotification
    {
        public DishCreateDto DishCreateDto { get; set; }
    }

    public class CreateDishCommandHandler : IRequestHandler<CreateDishCommand, DishResponseDto>
    {
        private readonly IDishRepository _dishRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;

        public CreateDishCommandHandler(
            IDishRepository dishRepository,
            IHotelRepository hotelRepository,
            IMapper mapper)
        {
            _dishRepository = dishRepository;
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        public async Task<DishResponseDto> Handle(CreateDishCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _hotelRepository.GetByIdAsync(request.DishCreateDto.HotelId);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.DishCreateDto.HotelId);
            }

            var dish = _mapper.Map<Dish>(request.DishCreateDto);
            var createdDish = await _dishRepository.AddAsync(dish);
            return _mapper.Map<DishResponseDto>(createdDish);
        }
    }
}

