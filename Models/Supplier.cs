using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("supplier")]
public class Supplier
{
    [Key]
    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Column("supplier_name")]
    public string? SupplierName { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("password")]
    public string? Password { get; set; }

    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    [Column("approval_status")]
    public string? ApprovalStatus { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
