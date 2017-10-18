using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpCommerce.Services
{
    using SharpCommerce.Data.Products;
    using SharpCommerce.Web;

    public class ProductCategoryService : Service
    {
        private const string BaseApiEndpoint = "products/categories";

        public ProductCategoryService(WoocommerceApiDriver apiDriver)
            : base(apiDriver) { }

        /// <summary>
        /// View a Product Categories
        /// </summary>
        /// <returns>a product categories</returns>
        public async Task<ProductCategory> Get(int productCategoryId)
        {
            var endPoint = String.Format("{0}/{1}", BaseApiEndpoint, productCategoryId);
            return (await Get<ProductCategoryBundle>(endPoint)).Content;
        }

        /// <summary>
        /// View a List of Product Categories
        /// </summary>
        /// <param name="parameters">Parameters to filter list of product categories</param>
        /// <returns>List of product categories</returns>
        public async Task<IEnumerable<ProductCategory>> Get(Dictionary<string, string> parameters = null)
        {
            return (await Get<ProductCategoriesBundle>(BaseApiEndpoint, parameters)).Content;
        }


    }
}
