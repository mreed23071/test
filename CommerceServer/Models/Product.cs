using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommerceServer.Models
{
    /// <summary>
    /// Order line modeled from the product table
    /// </summary>
    [Table("product")]
    public class Product
    {
        [Key] [Required] [Column("id")] public int Id { get; set; }
        [Column("name")] public string Name { get; set; }
        [Column("price")] public decimal Price { get; set; }
    }
}