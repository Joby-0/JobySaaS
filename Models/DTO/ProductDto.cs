using System.ComponentModel.DataAnnotations;

namespace Models.DTO;

public class ProductCreateRequest
{
    [Required]
    [StringLength(120)]
    public string ProductName { get; set; }

    [StringLength(500)]
    public string ProductDescription { get; set; }

    [Range(0, 1000000)]
    public decimal Price { get; set; }

    public Guid CategoryId { get; set; }
}

public class ProductUpdateRequest
{
    [Required]
    [StringLength(120)]
    public string ProductName { get; set; }

    [StringLength(500)]
    public string ProductDescription { get; set; }

    [Range(0, 1000000)]
    public decimal Price { get; set; }

    public bool IsActive { get; set; }
    public Guid CategoryId { get; set; }
}

public class ProductListDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}

public class ProductDetailDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
}

public class ResponsePageDto<T>
{
    public int DbItemsCount { get; set; }
    public int PageNr { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyList<T> PageItems { get; set; } = Array.Empty<T>();
}

public class ResponseItemDto<T>
{
    public T Item { get; set; }
}

public class LoginRequest
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}

public class LoginResponse
{
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public string UserRole { get; set; }
}

public class JwtUserToken
{
    public Guid TokenId { get; set; }
    public string EncryptedToken { get; set; }
    public DateTime ExpireTime { get; set; }
    public string UserRole { get; set; }
    public string UserName { get; set; }
    public Guid UserId { get; set; }
}
