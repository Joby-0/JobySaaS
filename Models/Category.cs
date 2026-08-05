namespace Models;

public class Category : ICategory
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategorySlug { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}
