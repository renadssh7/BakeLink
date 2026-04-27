using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("address")]
public class Address
{
    [Key]
    [Column("address_id")]
    public int AddressId { get; set; }

    [Column("supplier_id")]
    public int? SupplierId { get; set; }

    [Column("baker_id")]
    public int? BakerId { get; set; }

    [Column("city")]
    public string? City { get; set; }

    [Column("district")]
    public string? District { get; set; }

    [Column("street")]
    public string? Street { get; set; }

    [Column("latitude", TypeName = "decimal(10,7)")]
    public decimal? Latitude { get; set; }

    [Column("longitude", TypeName = "decimal(10,7)")]
    public decimal? Longitude { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public Supplier? Supplier { get; set; }

    [ForeignKey(nameof(BakerId))]
    public Bakery? Bakery { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
