using System.ComponentModel.DataAnnotations;

namespace Bake_Link.Validation;

public class CartItemRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
