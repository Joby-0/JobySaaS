namespace Models;

public class Product : IProduct
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
}
