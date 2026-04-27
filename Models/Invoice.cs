using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("invoice")]
public class Invoice
{
    [Key]
    [Column("invoice_id")]
    public int InvoiceId { get; set; }

    [Column("payment_id")]
    public int? PaymentId { get; set; }

    [Column("invoice_number")]
    public string? InvoiceNumber { get; set; }

    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    [ForeignKey(nameof(PaymentId))]
    public Payment? Payment { get; set; }
}
