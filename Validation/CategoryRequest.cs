using System.ComponentModel.DataAnnotations;

namespace Bake_Link.Validation;

public class CategoryRequest
{
    [Required]
    [StringLength(255)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [StringLength(255)]
    public string? Image { get; set; }
}
