using System.ComponentModel.DataAnnotations;

namespace DbModels;

public class CategoryDbM
{
    [Key]
    public Guid CategoryId { get; set; }

    [Required]
    [StringLength(120)]
    public string CategoryName { get; set; }

    [Required]
    [StringLength(120)]
    public string CategorySlug { get; set; }

    public ICollection<ProductDbM> Products { get; set; } = [];
}
