using MediatR;
using KingHotelProject.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace KingHotelProject.Application.Features.Dishes.Commands
{
    public class CacheInvalidationHandler :
        INotificationHandler<CreateDishCommand>,
        INotificationHandler<UpdateDishCommand>,
        INotificationHandler<DeleteDishCommand>
    {
        private readonly ICacheService _cacheService;
        private readonly IDishRepository _dishRepository;

        public CacheInvalidationHandler(
            ICacheService cacheService,
            IDishRepository dishRepository)
        {
            _cacheService = cacheService;
            _dishRepository = dishRepository;
        }

        public async Task Handle(CreateDishCommand notification, CancellationToken cancellationToken)
        {
            // Invalidate all dishes cache
            await _cacheService.RemoveAsync("AllDishes");

            // Invalidate hotel-specific dishes cache
            await _cacheService.RemoveAsync($"DishesByHotel_{notification.DishCreateDto.HotelId}");

            // Invalidate hotel cache if it exists
            await _cacheService.RemoveAsync($"Hotel_{notification.DishCreateDto.HotelId}");
        }

        public async Task Handle(UpdateDishCommand notification, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByIdAsync(notification.Id);
            if (dish != null)
            {
                // Invalidate all dishes cache
                await _cacheService.RemoveAsync("AllDishes");

                // Invalidate dish-specific cache
                await _cacheService.RemoveAsync($"Dish_{notification.Id}");

                // Invalidate hotel-specific dishes cache
                await _cacheService.RemoveAsync($"DishesByHotel_{dish.HotelId}");

                // Invalidate hotel cache if it exists
                await _cacheService.RemoveAsync($"Hotel_{dish.HotelId}");
            }
        }

        public async Task Handle(DeleteDishCommand notification, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByIdAsync(notification.Id);
            if (dish != null)
            {
                // Invalidate all dishes cache
                await _cacheService.RemoveAsync("AllDishes");

                // Invalidate dish-specific cache
                await _cacheService.RemoveAsync($"Dish_{notification.Id}");

                // Invalidate hotel-specific dishes cache
                await _cacheService.RemoveAsync($"DishesByHotel_{dish.HotelId}");

                // Invalidate hotel cache if it exists
                await _cacheService.RemoveAsync($"Hotel_{dish.HotelId}");
            }
        }
    }
}

