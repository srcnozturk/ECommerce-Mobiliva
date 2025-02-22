using AutoMapper;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Core.Entities;

namespace ECommerceAPI.Application.Mapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<CreateOrderRequest, OrderEntity>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src =>
                src.ProductDetails.Sum(p => p.UnitPrice * p.Amount)))
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.ProductDetails));

        CreateMap<ProductDetailDto, OrderDetailEntity>()
            .ForMember(dest => dest.ProductEntityId, opt => opt.MapFrom(src => src.ProjectId))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));
    }
}
