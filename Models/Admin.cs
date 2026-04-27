using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("admin")]
public class Admin
{
    [Key]
    [Column("admin_id")]
    public int AdminId { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
