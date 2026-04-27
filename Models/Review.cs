using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("review")]
public class Review
{
    [Key]
    [Column("review_id")]
    public int ReviewId { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }

    [Column("bakery_id")]
    public int? BakeryId { get; set; }

    [Column("supplier_id")]
    public int? SupplierId { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("rate")]
    public int? Rate { get; set; }

    [Column("review_date")]
    public DateTime? ReviewDate { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }

    [ForeignKey(nameof(BakeryId))]
    public Bakery? Bakery { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public Supplier? Supplier { get; set; }
}
