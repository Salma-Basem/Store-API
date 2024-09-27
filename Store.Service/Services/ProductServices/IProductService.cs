using Store.Repository.Specification.ProductSpecs;
using Store.Service.Helper;
using Store.Service.Services.ProductServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services
{
    public interface IProductService
    {
        Task<ProductDetailsDto> GetProductByIdAsync(int? productId);
        Task<PaginatedResultDto<ProductDetailsDto>> GetAllProductAsync(ProductSpecification specs);
        Task<IReadOnlyList<BrandTypeDetailsDto>> GetAllBrandsAsync();
        Task<IReadOnlyList<BrandTypeDetailsDto>> GetAllTypesAsync();

    }
}
