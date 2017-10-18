using System.Collections.Generic;

namespace SharpCommerce.Services
{
    using System;
    using SharpCommerce.Data.Orders;
    using SharpCommerce.Web;
    using System.Threading.Tasks;

    public class OrderNotesService : Service
    {
        public OrderNotesService(WoocommerceApiDriver apiDriver) : base(apiDriver) { }

        /// <summary>
        /// Create a Note For an Order
        /// </summary>
        /// <param name="orderId">The identifier of Order</param>
        /// <param name="newData">Order Note object to be created</param>
        /// <returns></returns>
        public async Task<OrderNote> Create(int orderId, OrderNote newData)
        {
            var endPoint = String.Format("orders/{0}/notes", orderId);
            var bundle = new OrderNoteBundle { Content = newData };
            return (await Post(endPoint, toSerialize: bundle)).Content;
        }

        /// <summary>
        /// View an Order Note
        /// </summary>
        /// <param name="orderId">The identifier of Order</param>
        /// <param name="noteId">The identifier of Note</param>
        /// <returns></returns>
        public async Task<OrderNote> Get(int orderId, int noteId)
        {
            var endPoint = String.Format("orders/{0}/notes/{1}", orderId, noteId);
            return (await Get<OrderNoteBundle>(endPoint)).Content;
        }

        /// <summary>
        /// View List Of Notes From An Order
        /// </summary>
        /// <param name="orderId">The identifier of Order</param>
        /// <returns></returns>
        public async Task<IEnumerable<OrderNote>> Get(int orderId)
        {
            var endPoint = String.Format("orders/{0}/notes", orderId);
            return (await Get<OrderNotesBundle>(endPoint)).Content;
        }

        /// <summary>
        /// Update An Order Note
        /// </summary>
        /// <param name="orderId">The identifier of Order</param>
        /// <param name="noteId">The identifier of Note</param>
        /// <param name="newData">Order Note object to be udpated</param>
        /// <returns></returns>
        public async Task<OrderNote> Update(int orderId, int noteId, OrderNote newData)
        {
            var endPoint = String.Format("orders/{0}/notes/{1}", orderId, noteId);
            var bundle = new OrderNoteBundle { Content = newData };
            return (await Put(endPoint, toSerialize: bundle)).Content;
        }

        /// <summary>
        /// Delete An Order Note
        /// </summary>
        /// <param name="orderId">The identifier of Order</param>
        /// <param name="noteId">The identifier of Note</param>
        /// <returns></returns>
        public async Task<string> Delete(int orderId, int noteId)
        {
            var endpoint = String.Format("orders/{0}/notes/{1}", orderId, noteId);
            return (await Delete<dynamic>(endpoint)).message;
        }
    }
}
