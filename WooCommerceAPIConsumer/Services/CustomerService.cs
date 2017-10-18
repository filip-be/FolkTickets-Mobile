using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpCommerce.Services
{
    using SharpCommerce.Data.Customers;
    using SharpCommerce.Data.Downloads;
    using SharpCommerce.Data.Orders;
    using SharpCommerce.Web;

    public class CustomerService : Service
    {
        public CustomerService(WoocommerceApiDriver apiDriver)
            : base(apiDriver) { }

        /// <summary>
        /// Create a Customer
        /// </summary>
        /// <param name="orderData"></param>
        /// <returns></returns>
        public async Task<Customer> Create(Customer orderData)
        {
            var bundle = new CustomerBundle { Content = orderData };
            return (await Post(apiEndpoint: "customers", toSerialize: bundle)).Content;
        }

        /// <summary>
        /// View a Customer by ID
        /// </summary>
        /// <param name="customerId">The identifier of customer</param>
        /// <returns></returns>
        public async Task<Customer> Get(int customerId)
        {
            var endPoint = string.Format("customers/{0}", customerId);
            return (await Get<CustomerBundle>(endPoint)).Content;
        }

        /// <summary>
        /// View a Customer by Email
        /// </summary>
        /// <param name="email">Email of customer</param>
        /// <returns></returns>
        public async Task<Customer> Get(string email)
        {
            var endPoint = string.Format("customers/email/{0}", email);
            return (await Get<CustomerBundle>(endPoint)).Content;
        }

        /// <summary>
        /// View List of Customers
        /// </summary>
        /// <param name="parameters">Parameters to filter list of customers</param>
        /// <returns></returns>
        public async Task<IEnumerable<Customer>> Get(Dictionary<string, string> parameters = null)
        {
            return (await Get<CustomersBundle>(apiEndpoint: "customers", parameters: parameters)).Content;
        }

        /// <summary>
        /// Update a Customer
        /// </summary>
        /// <param name="customerId">The identifier of customer</param>
        /// <param name="newData">Customer object to be updated</param>
        /// <returns></returns>
        public async Task<Customer> Update(int customerId, Customer newData)
        {
            var endPoint = String.Format("customers/{0}", customerId);
            var bundle = new CustomerBundle { Content = newData };
            return (await Put(endPoint, toSerialize: bundle)).Content;
        }

        /// <summary>
        /// Create or Update Multiple Customers
        /// </summary>
        /// <param name="ordersData">List of customer object to be created or updated</param>
        /// <returns></returns>
        public async Task<IEnumerable<Customer>> CreateUpdateMany(IEnumerable<Customer> ordersData)
        {
            var bundle = new CustomersBundle { Content = ordersData };
            return (await Put(apiEndpoint: "customers/bulk", toSerialize: bundle)).Content;
        }

        /// <summary>
        /// Delete a Customer Permanently
        /// </summary>
        /// <param name="id">The identifier of Customer</param>
        /// <returns></returns>
        public async Task<string> DeletePermanently(int id)
        {
            var endPoint = string.Format("customers/{0}", id);
            return (await Delete<dynamic>(endPoint)).message;
        }

        /// <summary>
        /// View Customers Count. 
        /// Can have the 'role' filter for customers vs subscribers
        /// </summary>
        /// <param name="parameters">Parameter to filter list of customer </param>
        /// <returns></returns>
        public async Task<int> Count(Dictionary<string, string> parameters = null)
        {
            return (await Get<dynamic>(apiEndpoint: "customers/count", parameters: parameters)).count;
        }

        /// <summary>
        /// View Customer's Order
        /// </summary>
        /// <param name="customerId">The identifier of customer</param>
        /// <param name="parameters">Parameter to filter list of orders</param>
        /// <returns></returns>
        public async Task<IEnumerable<Order>> GetOrders(int customerId, Dictionary<string, string> parameters = null)
        {
            var endPoint = String.Format("customers/{0}/orders", customerId);
            return (await Get<OrdersBundle>(endPoint, parameters: parameters)).Content;
        }

        /// <summary>
        /// View Customer's Download
        /// </summary>
        /// <param name="customerId">The identifier of customer</param>
        /// <param name="parameters">Parameter to filter list of downloads</param>
        /// <returns></returns>
        public async Task<IEnumerable<Download>> GetDownloads(int customerId, Dictionary<string, string> parameters = null)
        {
            var endPoint = String.Format("customers/{0}/downloads", customerId);
            return (await Get<DownloadsBundle>(endPoint, parameters: parameters)).Content;
        }
    }
}
