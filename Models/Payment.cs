using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("payment")]
public class Payment
{
    [Key]
    [Column("payment_id")]
    public int PaymentId { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }

    [Column("subtotal", TypeName = "decimal(18,2)")]
    public decimal? Subtotal { get; set; }

    [Column("tax", TypeName = "decimal(18,2)")]
    public decimal? Tax { get; set; }

    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    [Column("payment_method")]
    public string? PaymentMethod { get; set; }

    [Column("card_number")]
    public string? CardNumber { get; set; }

    [Column("card_holder_name")]
    public string? CardHolderName { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("ref_number")]
    public string? RefNumber { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }

    public Invoice? Invoice { get; set; }
}
