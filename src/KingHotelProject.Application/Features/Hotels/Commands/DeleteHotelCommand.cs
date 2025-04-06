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
            var hotel = await _hotelRepository.GetByIdAsync(request.Id);
            if (hotel == null)
            {
                throw new NotFoundException(nameof(Hotel), request.Id);
            }

            await _hotelRepository.DeleteAsync(hotel);

            // Invalidate caches
            await _cacheService.RemoveAsync("AllHotels");
            await _cacheService.RemoveAsync($"Hotel_{request.Id}");
            await _cacheService.RemoveAsync($"DishesByHotel_{request.Id}");
            await _cacheService.RemoveAsync("AllDishes"); // Since dishes related to this hotel will be deleted
        }
    }
}