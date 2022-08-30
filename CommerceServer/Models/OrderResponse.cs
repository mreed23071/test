using System.Collections.Generic;
using System.Linq;

namespace CommerceServer.Models
{
    public class OrderResponse
    {
        public IEnumerable<Transaction> Orders { get; set; }
        public decimal OrderAverage { get; set; }
        public int MostExpensiveOrderId { get; set; }
        public int LeastExpensiveOrderId { get; set; }
        public int TotalNumberOfOrders { get; set; }
        public int ResultsPerPage { get; set; }
        public int Page { get; set; }
        public string SortOrder { get; set; }

    }
}