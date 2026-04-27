using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("item")]
public class Item
{
    [Key]
    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("cart_id")]
    public int? CartId { get; set; }

    [Column("product_id")]
    public int? ProductId { get; set; }

    [Column("quantity")]
    public int? Quantity { get; set; }

    [Column("unit_price", TypeName = "decimal(18,2)")]
    public decimal? UnitPrice { get; set; }

    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    [ForeignKey(nameof(CartId))]
    public Cart? Cart { get; set; }
}
