using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpCommerce.Services
{
    using SharpCommerce.Data.Products;
    using SharpCommerce.Web;

    public class ProductReviewService : Service
    {
        private const string BaseApiEndpoint = "products";

        public ProductReviewService(WoocommerceApiDriver apiDriver)
            : base(apiDriver)
        {
        }


        /// <summary>
        /// View List Of Orders
        /// </summary>
        /// <param name="productId">A unique product identifier</param>
        /// <param name="parameters">parameter to filter list of product's reviews</param>
        /// <returns>List of product's review</returns>
        public async Task<IEnumerable<ProductReview>> Get(int productId, Dictionary<string, string> parameters = null)
        {
            var endPoint = String.Format("{0}/{1}/reviews", BaseApiEndpoint, productId);
            return (await Get<ProductReviewsBundle>(endPoint, parameters)).Content;
        }
    }
}
