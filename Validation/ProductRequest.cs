using System.ComponentModel.DataAnnotations;

namespace Bake_Link.Validation;

public class ProductRequest
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(255)]
    public string ProductName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public string? Images { get; set; }

    public bool ReplaceImages { get; set; }
}
