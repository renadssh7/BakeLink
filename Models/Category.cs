using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake_Link.Models;

[Table("category")]
public class Category
{
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("category_name")]
    public string? CategoryName { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("image")]
    public string? Image { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }

    [ForeignKey(nameof(AdminId))]
    public Admin? Admin { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
