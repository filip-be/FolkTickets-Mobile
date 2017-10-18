using System.Collections.Generic;

namespace SharpCommerce.Services
{
    using System;
    using SharpCommerce.Data.Orders;
    using SharpCommerce.Web;
    using System.Threading.Tasks;

    public class OrderService : Service
    {
        public readonly OrderNotesService Notes;
        public readonly OrderRefundsService Refunds;

        public OrderService(WoocommerceApiDriver apiDriver)
            : base(apiDriver)
        {
            Notes   = new OrderNotesService(apiDriver);
            Refunds = new OrderRefundsService(apiDriver);
        }

        /// <summary>
        /// Create an Order
        /// </summary>
        /// <param name="orderData">Order object to be sent.</param>
        /// <returns></returns>
        public async Task<Order> Create(Order orderData)
        {
            var bundle = new OrderBundle { Content = orderData };
            return (await Post(apiEndpoint: "orders", toSerialize: bundle)).Content;
        }

        /// <summary>
        /// View an Order
        /// </summary>
        /// <param name="orderId">The identifier of order</param>
        /// <returns>Order object</returns>
        public async Task<Order> Get(int orderId)
        {
            var endPoint = string.Format("orders/{0}", orderId);
            return (await Get<OrderBundle>(endPoint)).Content;
        }

        /// <summary>
        /// View List of Orders
        /// </summary>
        /// <param name="parameters">Parameters to filter list of orders</param>
        /// <returns>List of Orders</returns>
        public async Task<IEnumerable<Order>> Get(Dictionary<string, string> parameters = null)
        {
            return (await Get<OrdersBundle>(apiEndpoint: "orders", parameters: parameters)).Content;
        }
        
        /// <summary>
        /// Create or Update Multiple Orders
        /// </summary>
        /// <param name="ordersData">Orders object to be created or udpated</param>
        /// <returns></returns>
        public async Task<IEnumerable<Order>> CreateUpdateMany(IEnumerable<Order> ordersData)
        {
            var bundle = new OrdersBundle { Content = ordersData };
            return (await Put(apiEndpoint: "orders/bulk", toSerialize: bundle)).Content;
        }

        /// <summary>
        /// Update an Order
        /// </summary>
        /// <param name="orderId">The identifier of Order</param>
        /// <param name="newData">New data that will be updated</param>
        /// <returns>New Order Object</returns>
        public async Task<Order> Update(int orderId, Order newData)
        {
            var endPoint = String.Format("orders/{0}", orderId);
            var bundle = new OrderBundle { Content = newData };
            return (await Put(endPoint, toSerialize: bundle)).Content;
        }

        /// <summary>
        /// Delete an Order
        /// </summary>
        /// <param name="id">The identifier of Order</param>
        /// <returns></returns>
        public async Task<string> MoveToTrash(int id)
        {
            return await Delete(id);
        }

        /// <summary>
        /// Delete an Order Permanently
        /// </summary>
        /// <param name="id">The identifier of Order</param>
        /// <returns></returns>
        public async Task<string> DeletePermanently(int id)
        {
            return await Delete(id, force: true);
        }

        /// <summary>
        /// View Orders Count
        /// </summary>
        /// <param name="parameters">Parameter to filter the orders</param>
        /// <returns>Number of order's count</returns>
        public async Task<int> Count(Dictionary<string, string> parameters = null)
        {
            return (await Get<dynamic>("orders/count", parameters)).count; 
        }

        /// <summary>
        /// View List of Order Statuses
        /// </summary>
        /// <returns></returns>
        public async Task<OrderStatuses> GetStatuses()
        {
            return (await Get<OrderStatusesBundle>("orders/statuses")).Content;
        }
        
        private async Task<string> Delete(int id, bool force = false)
        {
            var apiEndpoint = String.Format("orders/{0}", id);
            var parameters = new Dictionary<string, string> { { "force", force.ToString().ToLower() } };
            return (await Delete<dynamic>(apiEndpoint, parameters)).message;
        }
    }
}
