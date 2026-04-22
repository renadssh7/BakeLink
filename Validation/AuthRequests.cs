using System.ComponentModel.DataAnnotations;

namespace Bake_Link.Validation;

public class LoginRequest
{
    [Required]
    [EmailAddress]       // abc@tho.com
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required]
    [RegularExpression("^(bakery|supplier)$")]
    public string Role { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^05[0-9]{8}$")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string District { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Street { get; set; }

    [Required]
    [StringLength(255)]
    public string BusinessName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? OrderVolume { get; set; }

    [StringLength(50)]
    public string? CommercialRegistration { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
