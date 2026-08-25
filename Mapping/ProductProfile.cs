
using AutoMapper;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;

namespace EcommerceApi.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, ProductDto>()
                .ForMember(
                    (dest) => dest.CategoryName, 
                    (opt) => opt.MapFrom(src => src.Category.Name)
                )
                .ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
        }
    }
}