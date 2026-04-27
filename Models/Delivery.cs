using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("delivery")]
public class Delivery
{
    [Key]
    [Column("delivery_id")]
    public int DeliveryId { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }

    [Column("delivery_status")]
    public string? DeliveryStatus { get; set; }

    [Column("delivery_date")]
    public DateTime? DeliveryDate { get; set; }

    [Column("delivery_cost", TypeName = "decimal(18,2)")]
    public decimal? DeliveryCost { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }
}
