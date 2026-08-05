namespace Models;

public interface IProduct
{
    Guid ProductId { get; set; }
    string ProductName { get; set; }
    string ProductDescription { get; set; }
    decimal Price { get; set; }
    bool IsActive { get; set; }
}
