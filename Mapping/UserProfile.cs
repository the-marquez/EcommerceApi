
using AutoMapper;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;

namespace EcommerceApi.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
            CreateMap<ApplicationUser, CreateUserDto>().ReverseMap();
            CreateMap<ApplicationUser, UserLoginDto>().ReverseMap();
            CreateMap<ApplicationUser, UserLoginResponseDto>().ReverseMap();
            CreateMap<ApplicationUser, UserDataDto>().ReverseMap();
        }
    }
}