using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommerceServer.Models
{
    /// <summary>
    /// Order line modeled from the commerce_transaction table
    /// </summary>
    [Table("commerce_transaction")]
    public class Transaction
    {
        [Key] [Required] [Column("id")] public int Id { get; set; }
        [Column("first_name")] public string FirstName { get; set; }
        [Column("last_name")] public string LastName { get; set; }
        [Column("email")] public string Email { get; set; }
        [Column("gender")] public string Gender { get; set; }
        [Column("address")] public string Address { get; set; }
        [Column("postal_code")] public string PostalCode { get; set; }
        [NotMapped] public decimal OrderTotal { get; set; }
        public IEnumerable<TransactionLine> TransactionLines { get; set; }
    }
}