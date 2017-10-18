using System.Collections.Generic;

namespace SharpCommerce.Services
{
    using System;
    using SharpCommerce.Data.Orders;
    using SharpCommerce.Web;
    using System.Threading.Tasks;

    public class OrderRefundsService : Service
    {
        public OrderRefundsService(WoocommerceApiDriver apiDriver) : base(apiDriver) { }

        /// <summary>
        /// Create A Refund For An Order
        /// </summary>
        /// <param name="orderId">The identifier of Order</param>
        /// <param name="newData">Order Refund object to be created</param>
        /// <returns></returns>
        public async Task<OrderRefund> Create(int orderId, OrderRefund newData)
        {
            return (await Post(apiEndpoint: String.Format("orders/{0}/refunds", orderId), toSerialize: new OrderRefundBundle() { Content = newData })).Content;
        }

        /// <summary>
        /// View An Order Note
        /// </summary>
        /// <param name="orderId">The identifier of Order</param>
        /// <param name="refundId">The identifier of Refund</param>
        /// <returns></returns>
        public async Task<OrderRefund> Get(int orderId, int refundId)
        {
            var endPoint = string.Format("orders/{0}/refunds/{1}", orderId, refundId);
            return (await Get<OrderRefundBundle>(endPoint)).Content;
        }

        /// <summary>
        /// View List of Refunds From An Order
        /// </summary>
        /// <param name="orderId">The identifier of Order</param>
        /// <returns></returns>
        public async Task<IEnumerable<OrderRefund>> Get(int orderId)
        {
            var endPoint = string.Format("orders/{0}/refunds", orderId);
            return (await Get<OrderRefundsBundle>(endPoint)).Content;
        }

        /// <summary>
        /// Update An Order Refund
        /// </summary>
        /// <param name="orderId">The identifier of Order Refund</param>
        /// <param name="refundId">The identifier of Refund</param>
        /// <param name="newData">Order Refund object to be updated</param>
        /// <returns></returns>
        public async Task<OrderRefund> Update(int orderId, int refundId, OrderRefund newData)
        {
            var endPoint = string.Format("orders/{0}/refunds/{1}", orderId, refundId);
            var bundle = new OrderRefundBundle { Content = newData };
            return (await Put(endPoint, toSerialize: bundle)).Content;
        }

        /// <summary>
        /// Delete An Order Refund
        /// </summary>
        /// <param name="orderId">The identifier of Order</param>
        /// <param name="refundId">The identifier of Refund</param>
        /// <returns></returns>
        public async Task<string> Delete(int orderId, int refundId)
        {
            var endPoint = String.Format("orders/{0}/refunds/{1}", orderId, refundId);
            return (await Delete<dynamic>(endPoint)).message;
        }
    }
}
