
using AutoMapper;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;

namespace EcommerceApi.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<User, UserLoginResponseDto>().ReverseMap();
            
        }
    }
}