using AutoMapper;
using EFCoreSample.Domain.Model;
using EFCoreSample.Dto;

namespace EFCoreSample.Mapping;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<CartDto, Cart>()
			.PreserveTracking()
			.PreserveReferences()
			.ReverseMap();

		CreateMap<ProductDto, Product>()
			.PreserveTracking()
      .PreserveReferences()
      .ReverseMap();

		CreateMap<OrderDto, Order>()
			.PreserveTracking()
      .PreserveReferences()
      .ReverseMap();
	}
}
