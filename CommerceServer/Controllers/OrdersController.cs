using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CommerceServer.DAL;
using CommerceServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

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
            var totalTransactions = await GetTransactionsCount();
            return ProcessOrders(transactions, page, rowsPerPage, sortOrder, totalTransactions);
        }

        /// <summary>
        /// Processes order data
        /// </summary>
        /// <param name="transactions">Current set of transactions from the database</param>
        /// <param name="page">Current page</param>
        /// <param name="rowsPerPage">Number of rows to return</param>
        /// <param name="sortOrder">Defines the sort for the results</param>
        /// <returns>Order response object with all properties filled</returns>
        private OrderResponse ProcessOrders(IReadOnlyCollection<Transaction> transactions, int page, int rowsPerPage, string sortOrder, int totalTransactions)
        {
            /* Transactions do not have an order total set in the database.
                We need to calculate the order here. it'll be the sum of transactionLine Quantity * TransactionLine product price
                You must use LINQ to complete this exercise. 
                TODO: Add calculation for each transactions order total
             */
            IEnumerable<Transaction> orders = from transaction in transactions select new Transaction
            {
                Id = transaction.Id,
                FirstName = transaction.FirstName,
                LastName = transaction.LastName,
                Email = transaction.Email,
                Gender = transaction.Gender,
                Address = transaction.Address,
                PostalCode = transaction.PostalCode,
                OrderTotal = (from line in transaction.TransactionLines select line.Quantity * line.Product.Price).Sum(),
                TransactionLines = transaction.TransactionLines
            };

            var orderResponse = new OrderResponse()
            {
                Orders = orders, //TODO: Add transactions here
                OrderAverage = (from transaction in orders select transaction.OrderTotal).Average(), //TODO: Calculate the order average
                LeastExpensiveOrderId = (
                from order in orders
                where order.OrderTotal == orders.Min(order => order.OrderTotal) 
                select order.Id).FirstOrDefault(), //TODO: Add least expensive order within the result set
                MostExpensiveOrderId = (
                from order in orders
                where order.OrderTotal == orders.Max(order => order.OrderTotal)
                select order.Id).FirstOrDefault(), //TODO: Add most expensive order within the result set
                TotalNumberOfOrders = totalTransactions, //TODO: Get total number of transactions (not just within result set)
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
            /* Add your code below to implement the sort order */
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    //TODO: Add Ascending sort logic
                    query = from transaction in query orderby transaction.Id select transaction;
                    break;
                case SortOrder.Descending:
                    //TODO: Add Descending sort logic
                    query = from transaction in query orderby transaction.Id descending select transaction;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Invalid sort order");
            }
            //TODO: Add your code here to implement the pagination
            List<Transaction> transactionPage =  query.Skip(rowsPerPage * (page - 1)).Take(rowsPerPage).ToList();
            return await Task.FromResult(transactionPage);
        }

        /// <summary>
        /// Gets total number of transactions from the database
        /// </summary>
        private async Task<int> GetTransactionsCount()
        {
            //This query will call the database and return transactions while including the Product on the Transaction
            var query = _context.Customer.Include("TransactionLines.Product"); //Don't change this line
                                                                               //Get Transaction Count
            int transactionPageCount = (from transaction in query select transaction).Count();
            return await Task.FromResult(transactionPageCount);
        }
    }
}
