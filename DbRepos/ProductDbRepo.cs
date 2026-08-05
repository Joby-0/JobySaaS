using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Models.DTO;

namespace DbRepos;

public class ProductDbRepo
{
    readonly ReferenceDbContext _dbContext;

    public ProductDbRepo(ReferenceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResponsePageDto<ProductListDto>> ReadProductsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Select(x => new ProductListDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                CategoryName = x.Category.CategoryName,
                Price = x.Price,
                IsActive = x.IsActive
            });

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new ResponsePageDto<ProductListDto>
        {
            DbItemsCount = totalCount,
            PageNr = pageNumber,
            PageSize = pageSize,
            PageItems = items
        };
    }

    public async Task<ResponseItemDto<ProductDetailDto>> ReadProductAsync(Guid id)
    {
        var item = await _dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.ProductId == id)
            .Select(x => new ProductDetailDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductDescription = x.ProductDescription,
                Price = x.Price,
                IsActive = x.IsActive,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.CategoryName
            })
            .FirstOrDefaultAsync();

        return item == null ? null : new ResponseItemDto<ProductDetailDto> { Item = item };
    }

    public async Task<ResponseItemDto<ProductDetailDto>> CreateProductAsync(ProductCreateRequest request)
    {
        var entity = new ProductDbM
        {
            ProductId = Guid.NewGuid(),
            ProductName = request.ProductName,
            ProductDescription = request.ProductDescription,
            Price = request.Price,
            IsActive = true,
            CategoryId = request.CategoryId
        };

        _dbContext.Products.Add(entity);
        await _dbContext.SaveChangesAsync();

        var detail = await ReadProductAsync(entity.ProductId);
        return detail;
    }

    public async Task<ResponseItemDto<ProductDetailDto>> UpdateProductAsync(Guid id, ProductUpdateRequest request)
    {
        var entity = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
        if (entity == null)
            return null;

        entity.ProductName = request.ProductName;
        entity.ProductDescription = request.ProductDescription;
        entity.Price = request.Price;
        entity.IsActive = request.IsActive;
        entity.CategoryId = request.CategoryId;

        await _dbContext.SaveChangesAsync();
        return await ReadProductAsync(id);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var entity = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
        if (entity == null)
            return false;

        _dbContext.Products.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
