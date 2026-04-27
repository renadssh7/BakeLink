using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("message")]
public class Message
{
    [Key]
    [Column("message_id")]
    public int MessageId { get; set; }

    [Column("sender_type")]
    public string? SenderType { get; set; }

    [Column("sender_id")]
    public int? SenderId { get; set; }

    [Column("receiver_type")]
    public string? ReceiverType { get; set; }

    [Column("receiver_id")]
    public int? ReceiverId { get; set; }

    [Column("message_content")]
    public string? MessageContent { get; set; }

    [Column("sent_at")]
    public DateTime? SentAt { get; set; }
}
