using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Bake_Link.Models;
//read
[Table("product")]
public class Product
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("supplier_id")]
    public int? SupplierId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("product_name")]
    public string? ProductName { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("price", TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }

    [Column("stock_quantity")]
    public int? StockQuantity { get; set; }

    [Column("images")]
    public string? ImagesJson { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public Supplier? Supplier { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();

    [NotMapped]
    public List<string> Images
    {
        get => string.IsNullOrWhiteSpace(ImagesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(ImagesJson) ?? new List<string>();
        set => ImagesJson = JsonSerializer.Serialize(value);
    }
}
