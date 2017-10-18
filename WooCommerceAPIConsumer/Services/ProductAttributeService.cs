using System;
using System.Collections.Generic;

namespace SharpCommerce.Services
{
    using SharpCommerce.Data.Products;
    using SharpCommerce.Web;
    using System.Threading.Tasks;

    public class ProductAttributeService : Service
    {
        private const string BaseApiEndpoint = "products/attributes";

        public ProductAttributeService(WoocommerceApiDriver apiDriver)
            : base(apiDriver) { }

        /// <summary>
        /// Create a Product Attribute 
        /// </summary>
        /// <param name="productAttributeData">Product attribute to be created</param>
        /// <returns>Newly created product attribute object</returns>
        public async Task<ProductAttribute> Create(ProductAttribute productAttributeData)
        {
            var bundle = new ProductAttributeBundle { Content = productAttributeData };
            return (await Post(apiEndpoint: BaseApiEndpoint, toSerialize: bundle)).Content;
        }

        /// <summary>
        /// View a Product Attribute
        /// </summary>
        /// <param name="productAttributeId">The identifiter of product attribute</param>
        /// <returns>A product attribute object</returns>
        public async Task<ProductAttribute> Get(int productAttributeId)
        {
            var endPoint = String.Format("{0}/{1}", BaseApiEndpoint, productAttributeId);
            return (await Get<ProductAttributeBundle>(endPoint)).Content;
        }

        /// <summary>
        /// View List of Product Attributes
        /// </summary>
        /// <param name="parameters">Parameter to filter list of product attributes</param>
        /// <returns>List of product attributes object</returns>
        public async Task<IEnumerable<ProductAttribute>> Get(Dictionary<string, string> parameters = null)
        {
            return (await Get<ProductAttributesBundle>(apiEndpoint: BaseApiEndpoint, parameters: parameters)).Content;
        }

        /// <summary>
        /// Update a product attribute
        /// </summary>
        /// <param name="productAttributeId">The identifier of product attribute</param>
        /// <param name="newData">The new data to be updated</param>
        /// <returns>New product attribute object</returns>
        public async Task<ProductAttribute> Update(int productAttributeId, ProductAttribute newData)
        {
            var endPoint = String.Format("{0}/{1}", BaseApiEndpoint, productAttributeId);
            var bundle = new ProductAttributeBundle { Content = newData };
            return (await Put(endPoint, toSerialize: bundle)).Content;
        }

        /// <summary>
        /// Delete a product attribute
        /// </summary>
        /// <param name="id">The identifier of product attribute</param>
        /// <returns></returns>
        public async Task<string> MoveToTrash(int id)
        {
            return await Delete(id);
        }

        /// <summary>
        /// Delete a Product Attribute Permanently
        /// </summary>
        /// <param name="id">The identifier of product attribute</param>
        /// <returns></returns>
        public async Task<string> DeletePermanently(int id)
        {
            return await Delete(id, force: true);
        }

        private async Task<string> Delete(int id, bool force = false)
        {
            var apiEndpoint = String.Format("{0}/{1}", BaseApiEndpoint, id);
            var parameters = new Dictionary<string, string> { { "force", force.ToString().ToLower() } };
            return(await  Delete<dynamic>(apiEndpoint, parameters)).message;
        }
    }
}
