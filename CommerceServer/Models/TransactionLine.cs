using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommerceServer.Models
{
    /// <summary>
    /// Order line modeled from the commerce_transaction_line table
    /// </summary>
    [Table("commerce_transaction_line")]
    public class TransactionLine
    {
        [Key] [Required] [Column("id")] public int Id { get; set; }
        [ForeignKey("Customer")] [Column("transaction_id")] public int TransactionId { get; set; }
        [ForeignKey("Product")] [Column("product_id")] public int ProductId { get; set; }
        [Column("Quantity")] public int Quantity { get; set; }
        public virtual Product Product { get; set; }
    }
}