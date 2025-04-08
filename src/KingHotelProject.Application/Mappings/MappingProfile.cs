using AutoMapper;
using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.DTOs.Users;
using KingHotelProject.Core.Entities;

namespace KingHotelProject.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<User, UserResponseDto>().ReverseMap();

            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Hotel Mappings
            CreateMap<Hotel, HotelResponseDto>()
                .ForMember(dest => dest.Dishes, opt => opt.MapFrom(src => src.Dishes))
                .ReverseMap();

            CreateMap<HotelCreateDto, Hotel>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Dishes, opt => opt.Ignore());

            CreateMap<HotelUpdateDto, Hotel>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Dishes, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Hotel Bulk Mapping 
            CreateMap<HotelsBulkCreateDto, Hotel>()
                .ForMember(dest => dest.HotelId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Dishes, opt => opt.Ignore());

            // Dish Mappings
            CreateMap<Dish, DishResponseDto>().ReverseMap();

            CreateMap<DishCreateDto, Dish>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Hotel, opt => opt.Ignore());

            CreateMap<DishUpdateDto, Dish>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.HotelId, opt => opt.Ignore())
                .ForMember(dest => dest.Hotel, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Dish Bulk Mapping
            CreateMap<DishesBulkCreateDto, Dish>()
                .ForMember(dest => dest.DishId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Hotel, opt => opt.Ignore());
        }
    }
}