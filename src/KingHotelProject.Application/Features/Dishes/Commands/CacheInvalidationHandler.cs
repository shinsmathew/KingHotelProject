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
            await InvalidateCache(notification.DishCreateDto.HotelId);
        }

        public async Task Handle(UpdateDishCommand notification, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByIdAsync(notification.Id);
            await InvalidateCache(dish.HotelId);
        }

        public async Task Handle(DeleteDishCommand notification, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByIdAsync(notification.Id);
            await InvalidateCache(dish.HotelId);
        }

        private async Task InvalidateCache(Guid hotelId)
        {
            await _cacheService.RemoveAsync("AllDishes");
            await _cacheService.RemoveAsync($"DishesByHotel_{hotelId}");
            // Invalidate other related cache keys if needed
        }
    }
}