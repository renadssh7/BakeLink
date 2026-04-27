using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("bakery")]
public class Bakery
{
    [Key]
    [Column("bakery_id")]
    public int BakeryId { get; set; }

    [Column("bakery_name")]
    public string? BakeryName { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("password")]
    public string? Password { get; set; }

    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
