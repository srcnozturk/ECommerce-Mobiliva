using AutoMapper;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Core.Entities;

namespace ECommerceAPI.Application.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductEntity, ProductDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));
    }
}
