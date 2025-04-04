
using AutoMapper;
using KingHotelProject.Application.DTOs;
using KingHotelProject.Core.Entities;

namespace KingHotelProject.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponseDto>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<Hotel, HotelResponseDto>();
            CreateMap<HotelCreateDto, Hotel>();
            CreateMap<HotelUpdateDto, Hotel>();
            CreateMap<Dish, DishResponseDto>();
            CreateMap<DishCreateDto, Dish>();
            CreateMap<DishUpdateDto, Dish>();
        }
    }
}