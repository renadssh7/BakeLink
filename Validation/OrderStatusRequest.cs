using System.ComponentModel.DataAnnotations;

namespace Bake_Link.Validation;

public class OrderStatusRequest
{
    [Required]
    public string OrderStatus { get; set; } = string.Empty;
}
