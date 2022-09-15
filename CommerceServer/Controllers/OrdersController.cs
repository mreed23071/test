using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommerceServer.DAL;
using CommerceServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommerceServer.Controllers
{
    /// <summary>
    /// Gets orders from database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Represents the number of orders to return from the API
        /// </summary>
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/Orders
        /// Gets orders from database.
        /// TODO: Implement query to retrieve required data from database
        /// TODO: Implement Order Response and return data as required
        /// </summary>
        /// <param name="sortOrder">Specifies the sort order of the data to be returned</param>
        /// <param name="page">Specifies which page to return</param>
        /// <param name="rowsPerPage">The number of orders in the page</param>
        /// <returns>Restful order response</returns>
        [HttpGet]
        public async Task<OrderResponse> Get([FromQuery] string sortOrder = "asc", [FromQuery] int page = 1, [FromQuery] int rowsPerPage = 25)
        {
            var transactions = await GetTopTransactions(sortOrder, page, rowsPerPage);
            return ProcessOrders(transactions, page, rowsPerPage, sortOrder);
        }

        /// <summary>
        /// Processes order data
        /// </summary>
        /// <param name="transactions">Current set of transactions from the database</param>
        /// <param name="page">Current page</param>
        /// <param name="rowsPerPage">Number of rows to return</param>
        /// <param name="sortOrder">Defines the sort for the results</param>
        /// <returns>Order response object with all properties filled</returns>
        private OrderResponse ProcessOrders(IReadOnlyCollection<Transaction> transactions, int page, int rowsPerPage, string sortOrder)
        {
            /* Transactions do not have an order total set in the database.
                We need to calculate the order here. it'll be the sum of transactionLine Quantity * TransactionLine product price
                You must use LINQ to complete this exercise. 
                TODO: Add calculation for each transactions order total
             */
            var query = _context.Customer.Include("TransactionLines.Product"); //Don't change this line
            var trs = transactions.Select(t => { t.OrderTotal = t.TransactionLines.Select(tl => tl.Quantity * tl.Product.Price).Sum(); return t; });
            int leastExpensiveOrderId, mostExpensiveOrderId;
            decimal orderAverage;
            if (transactions.Count() == 0)
            {

                orderAverage = 0;
                leastExpensiveOrderId = 0;
                mostExpensiveOrderId = 0;
            }
            else
            {
                orderAverage = Math.Round((from t in trs select t.OrderTotal).ToList().Average(), 2);
                leastExpensiveOrderId = (from t in trs orderby t.OrderTotal select t.Id).ToArray()[0];
                mostExpensiveOrderId = (from t in trs orderby t.OrderTotal descending select t.Id).ToArray()[0];

            }
            

            var orderResponse = new OrderResponse()
            {

                Orders = trs, //TODO: Add transactions here
                OrderAverage = orderAverage, //TODO: Calculate the order average
                LeastExpensiveOrderId = leastExpensiveOrderId, //TODO: Add least expensive order within the result set
                MostExpensiveOrderId = mostExpensiveOrderId, //TODO: Add most expensive order within the result set
                //TotalNumberOfOrders = trs.Count(), //TODO: Get total number of transactions (not just within result set)
                TotalNumberOfOrders = query.Count(), //TODO: Get total number of transactions (not just within result set)
                ResultsPerPage = rowsPerPage, //Rows per page
                Page = page, //Current page
                SortOrder = sortOrder //Current sort Order
            };
            return orderResponse;
        }

        /// <summary>
        /// Gets transactions from the database within the requested parameters
        /// </summary>
        /// <param name="sortOrder">Specified sort order from request</param>
        /// <param name="page">Specified page from request</param>
        /// <param name="rowsPerPage">Number of rows to return</param>
        /// <returns>Collection of transactions</returns>
        private async Task<IReadOnlyCollection<Transaction>> GetTopTransactions(string sortOrder, int page, int rowsPerPage)
        {
            //This query will call the database and return transactions while including the Product on the Transaction
            var query = _context.Customer.Include("TransactionLines.Product"); //Don't change this line
            List<Transaction> transactions = new List<Transaction>();
            /* Add your code below to implement the sort order */
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    //TODO: Add Ascending sort logic
                    transactions = (from t in query orderby t.Id select t).Skip((page - 1) * rowsPerPage).Take(rowsPerPage).ToList();
                    //transactions = (from t in query where t.OrderTotal != 0 select t).Skip((page - 1) * rowsPerPage).Take(rowsPerPage).ToList();
                    break;
                case SortOrder.Descending:
                    //TODO: Add Descending sort logic
                    transactions = (from t in query orderby t.Id descending select t).Skip((page - 1) * rowsPerPage).Take(rowsPerPage).ToList();
                    break;
                default:
                    throw new InvalidEnumArgumentException("Invalid sort order");
            }
            //TODO: Add your code here to implement the pagination

            return await Task.FromResult(transactions);
        }
    }
}
