using System.ComponentModel.DataAnnotations;

namespace Bake_Link.Validation;

public class ReviewRequest
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public int SupplierId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rate { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }
}
