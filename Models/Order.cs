using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("order")]
public class Order
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("bakery_id")]
    public int? BakeryId { get; set; }

    [Column("supplier_id")]
    public int? SupplierId { get; set; }

    [Column("address_id")]
    public int? AddressId { get; set; }

    [Column("order_date")]
    public DateTime? OrderDate { get; set; }

    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    [Column("order_status")]
    public string? OrderStatus { get; set; }

    [Column("estimated_delivery_cost", TypeName = "decimal(18,2)")]
    public decimal? EstimatedDeliveryCost { get; set; }

    [ForeignKey(nameof(BakeryId))]
    public Bakery? Bakery { get; set; }

    [ForeignKey(nameof(AddressId))]
    public Address? Address { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public Supplier? Supplier { get; set; }

    public Cart? Cart { get; set; }

    public Delivery? Delivery { get; set; }

    public Payment? Payment { get; set; }

    [NotMapped]
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
