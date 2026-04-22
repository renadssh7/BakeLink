using System.ComponentModel.DataAnnotations;

namespace Bake_Link.Validation;

public class CheckoutRequest
{
    public int? AddressId { get; set; }

    public bool NewAddress { get; set; }

    [StringLength(255)]
    public string? BakeryName { get; set; }

    [StringLength(10)]
    public string? ContactNumber { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? District { get; set; }

    [StringLength(255)]
    public string? Street { get; set; }

    [StringLength(50)]
    public string? BuildingNumber { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Instructions { get; set; }

    [Required]
    [StringLength(255)]
    public string CardHolder { get; set; } = string.Empty;

    [Required]
    public string CardNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string ExpiryMonth { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string ExpiryYear { get; set; } = string.Empty;

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Cvv { get; set; } = string.Empty;
}
