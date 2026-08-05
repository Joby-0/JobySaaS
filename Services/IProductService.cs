using Models.DTO;

namespace Services;
public class Test
{
}
public interface IProductService
{
    Task<ResponsePageDto<ProductListDto>> ReadProductsAsync(int pageNumber, int pageSize);
    Task<ResponseItemDto<ProductDetailDto>> ReadProductAsync(Guid id);
    Task<ResponseItemDto<ProductDetailDto>> CreateProductAsync(ProductCreateRequest request);
    Task<ResponseItemDto<ProductDetailDto>> UpdateProductAsync(Guid id, ProductUpdateRequest request);
    Task<bool> DeleteProductAsync(Guid id);
}
