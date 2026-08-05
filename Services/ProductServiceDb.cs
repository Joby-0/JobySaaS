using DbRepos;
using Models.DTO;

namespace Services;

public class ProductServiceDb : IProductService
{
    readonly ProductDbRepo _repo;

    public ProductServiceDb(ProductDbRepo repo)
    {
        _repo = repo;
    }

    public Task<ResponsePageDto<ProductListDto>> ReadProductsAsync(int pageNumber, int pageSize)
        => _repo.ReadProductsAsync(pageNumber, pageSize);

    public Task<ResponseItemDto<ProductDetailDto>> ReadProductAsync(Guid id)
        => _repo.ReadProductAsync(id);

    public Task<ResponseItemDto<ProductDetailDto>> CreateProductAsync(ProductCreateRequest request)
        => _repo.CreateProductAsync(request);

    public Task<ResponseItemDto<ProductDetailDto>> UpdateProductAsync(Guid id, ProductUpdateRequest request)
        => _repo.UpdateProductAsync(id, request);

    public Task<bool> DeleteProductAsync(Guid id)
        => _repo.DeleteProductAsync(id);
}
