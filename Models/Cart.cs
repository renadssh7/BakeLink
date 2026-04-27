using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("cart")]
public class Cart
{
    [Key]
    [Column("cart_id")]
    public int CartId { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }

    [Column("quantity")]
    public int? Quantity { get; set; }

    [Column("unit_price", TypeName = "decimal(18,2)")]
    public decimal? UnitPrice { get; set; }

    [Column("subtotal", TypeName = "decimal(18,2)")]
    public decimal? Subtotal { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();
}
