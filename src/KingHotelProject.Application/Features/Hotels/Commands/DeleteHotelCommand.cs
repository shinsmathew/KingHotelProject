using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using MediatR;

namespace KingHotelProject.Application.Features.Hotels.Commands
{
    public class DeleteHotelCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteHotelCommandHandler : IRequestHandler<DeleteHotelCommand>
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly ICacheService _cacheService;

        public DeleteHotelCommandHandler(
            IHotelRepository hotelRepository,
            ICacheService cacheService)
        {
            _hotelRepository = hotelRepository;
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(request.Id);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.Id);
            }

            await _hotelRepository.DeleteHotelAsync(hotel);

            // Invalidate caches
            await _cacheService.RemoveRedisCacheAsync("AllHotels");
            await _cacheService.RemoveRedisCacheAsync($"Hotel_{request.Id}");
            await _cacheService.RemoveRedisCacheAsync($"DishesByHotel_{request.Id}");
            await _cacheService.RemoveRedisCacheAsync("AllDishes"); 
        }
    }
}