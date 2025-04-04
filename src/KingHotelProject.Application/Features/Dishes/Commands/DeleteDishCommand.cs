// KingHotelProject.Application/Features/Dishes/Commands/DeleteDishCommand.cs
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

        public DeleteDishCommandHandler(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        public async Task Handle(DeleteDishCommand request, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByIdAsync(request.Id);
            if (dish == null)
            {
                throw new NotFoundException(nameof(Dish), request.Id);
            }

            await _dishRepository.DeleteAsync(dish);
        }
    }
}