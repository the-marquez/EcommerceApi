
using AutoMapper;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;

namespace EcommerceApi.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
        }
    }
}