using CommerceServer.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceServer.DAL
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Product> Product { get; set; }
        public DbSet<Transaction> Customer { get; set; }
        public DbSet<TransactionLine> CustOrder { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}