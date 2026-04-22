using System.ComponentModel.DataAnnotations;

namespace Bake_Link.Validation;

public class BakeryProfileRequest
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [RegularExpression("^05[0-9]{8}$")]
    public string? Phone { get; set; }
}

public class SupplierProfileRequest
{
    [Required]
    [StringLength(255)]
    public string SupplierName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [RegularExpression("^05[0-9]{8}$")]
    public string? PhoneNumber { get; set; }
}

public class AdminProfileRequest
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class PasswordUpdateRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class DeleteAccountRequest
{
    [Range(typeof(bool), "true", "true")]
    public bool ConfirmDelete { get; set; }

    [StringLength(500)]
    public string? DeleteReason { get; set; }
}
