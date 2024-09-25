using AutoMapper;
using AutoMapper.Configuration.Conventions;
using Microsoft.Extensions.Options;
using Store.Data.Entities;


namespace Store.Service.Services.ProductServices.Dtos
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDetailsDto>()
                .ForMember(dest => dest.BrandName, options => options.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.TypeName, options => options.MapFrom(src => src.Type.Name))
                .ForMember(dest => dest.PictureUrl, options => options.MapFrom<ProductPictureUrlResolver>());

            CreateMap<ProductBrand, BrandTypeDetailsDto>();
            CreateMap<ProductType, BrandTypeDetailsDto>();

        }
    }
}
