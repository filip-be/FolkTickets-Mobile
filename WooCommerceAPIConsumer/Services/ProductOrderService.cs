using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpCommerce.Services
{
    using SharpCommerce.Data.Orders;
    using SharpCommerce.Web;

    public class ProductOrderService : Service
    {
        private const string BaseApiEndpoint = "products";

        public ProductOrderService(WoocommerceApiDriver apiDriver)
            : base(apiDriver)
        {
        }

        /// <summary>
        /// View List Of Orders
        /// </summary>
        /// <param name="productId">A unique product identifier</param>
        /// <param name="parameters">parameter to filter list of product's order</param>
        /// <returns>List of product's order</returns>
        public async Task<IEnumerable<Order>> Get(int productId, Dictionary<string, string> parameters = null)
        {
            var endPoint = String.Format("{0}/{1}/orders", BaseApiEndpoint, productId);
            return (await Get<OrdersBundle>(endPoint, parameters)).Content;
        }
    }
}
