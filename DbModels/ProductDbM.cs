using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace DbModels;

public class ProductDbM
{
    [Key]
    public Guid ProductId { get; set; }

    [Required]
    [StringLength(120)]
    public string ProductName { get; set; }

    [StringLength(500)]
    public string ProductDescription { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public CategoryDbM Category { get; set; }
}
