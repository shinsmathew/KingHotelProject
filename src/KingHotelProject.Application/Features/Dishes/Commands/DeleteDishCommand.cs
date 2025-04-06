using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Dishes.Commands
{
    public class DeleteDishCommand : IRequest, INotification
    {
        public Guid Id { get; set; }
    }

    public class DeleteDishCommandHandler : IRequestHandler<DeleteDishCommand>
    {
        private readonly IDishRepository _dishRepository;
        private readonly ICacheService _cacheService;

        public DeleteDishCommandHandler(
            IDishRepository dishRepository,
            ICacheService cacheService)
        {
            _dishRepository = dishRepository;
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteDishCommand request, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByIdAsync(request.Id);
            if (dish == null)
            {
                throw new NotFoundException(nameof(Dish), request.Id);
            }

            // Store the hotelId before deleting the dish
            var hotelId = dish.HotelId;

            await _dishRepository.DeleteAsync(dish);

            // Invalidate relevant caches
            await _cacheService.RemoveAsync("AllDishes");
            await _cacheService.RemoveAsync($"DishesByHotel_{hotelId}");
            await _cacheService.RemoveAsync($"Dish_{request.Id}");
        }
    }
}

