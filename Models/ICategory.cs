namespace Models;

public interface ICategory
{
    Guid CategoryId { get; set; }
    string CategoryName { get; set; }
    string CategorySlug { get; set; }
}
