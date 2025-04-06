using MediatR;
using KingHotelProject.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace KingHotelProject.Application.Features.Hotels.Commands
{
    public class HotelCacheInvalidationHandler :
        INotificationHandler<CreateHotelCommand>,
        INotificationHandler<UpdateHotelCommand>,
        INotificationHandler<DeleteHotelCommand>
    {
        private readonly ICacheService _cacheService;

        public HotelCacheInvalidationHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task Handle(CreateHotelCommand notification, CancellationToken cancellationToken)
        {
            // Invalidate the AllHotels cache
            await _cacheService.RemoveAsync("AllHotels");
        }

        public async Task Handle(UpdateHotelCommand notification, CancellationToken cancellationToken)
        {
            // Invalidate caches
            await _cacheService.RemoveAsync("AllHotels");
            await _cacheService.RemoveAsync($"Hotel_{notification.Id}");

            // If the hotel has dishes, their cache might need to be invalidated too
            await _cacheService.RemoveAsync($"DishesByHotel_{notification.Id}");
        }

        public async Task Handle(DeleteHotelCommand notification, CancellationToken cancellationToken)
        {
            // Invalidate caches
            await _cacheService.RemoveAsync("AllHotels");
            await _cacheService.RemoveAsync($"Hotel_{notification.Id}");

            // If the hotel has dishes, their cache might need to be invalidated too
            await _cacheService.RemoveAsync($"DishesByHotel_{notification.Id}");
            await _cacheService.RemoveAsync("AllDishes"); // Since dishes related to this hotel will be deleted
        }
    }
}

