namespace SharpCommerce.Services
{
    using System;
    using System.Collections.Generic;

    using SharpCommerce.Data.Coupons;
    using SharpCommerce.Web;
    using System.Threading.Tasks;

    public class CouponService : Service
    {
        public CouponService(WoocommerceApiDriver apiDriver)
            : base(apiDriver) { }

        /// <summary>
        /// Create a Coupon
        /// </summary>
        /// <param name="orderData"></param>
        /// <returns></returns>
        public async Task<Coupon> Create(Coupon orderData)
        {
            var bundle = new CouponBundle { Content = orderData };
            return (await Post(apiEndpoint: "coupons", toSerialize: bundle)).Content;
        }

        /// <summary>
        /// View a Coupon by ID
        /// </summary>
        /// <param name="couponId">The identifier of Coupon</param>
        /// <returns>Specified coupon object</returns>
        public async Task<Coupon> Get(int couponId)
        {
            var endPoint = String.Format("coupons/{0}", couponId);
            return (await Get<CouponBundle>(endPoint)).Content;
        }

        /// <summary>
        /// View a Coupon by Code
        /// </summary>
        /// <param name="code">Code of Coupon</param>
        /// <returns>Specified coupon object</returns>
        public async Task<Coupon> Get(string code)
        {
            var endPoint = String.Format("coupons/code/{0}", code);
            return (await Get<CouponBundle>(endPoint)).Content;
        }

        /// <summary>
        /// List of Coupons
        /// </summary>
        /// <param name="parameters">Parameter to filter list of coupon</param>
        /// <returns>List of Coupon objects</returns>
        public async Task<IEnumerable<Coupon>> Get(Dictionary<string, string> parameters = null)
        {
            return (await Get<CouponsBundle>("coupons", parameters)).Content;
        }

        /// <summary>
        /// Update a Coupon
        /// </summary>
        /// <param name="couponId">The identifier of Coupon</param>
        /// <param name="newData">Coupon object to be updated</param>
        /// <returns>The new updated coupon</returns>
        public async Task<Coupon> Update(int couponId, Coupon newData)
        {
            var endPoint = String.Format("coupons/{0}", couponId);
            var bundle = new CouponBundle { Content = newData };
            return (await Put(endPoint, toSerialize: bundle)).Content;
        }

        /// <summary>
        /// Create or Update Multiple Coupon
        /// </summary>
        /// <param name="ordersData">Multiple coupon object to be created or updated</param>
        /// <returns></returns>
        public async Task<IEnumerable<Coupon>> CreateUpdateMany(IEnumerable<Coupon> ordersData)
        {
            var bundle = new CouponsBundle { Content = ordersData };
            return (await Put("coupons/bulk", toSerialize: bundle)).Content;
        }

        /// <summary>
        /// Delete a Coupon
        /// </summary>
        /// <param name="id">The identifier of coupon to be deleted</param>
        /// <returns></returns>
        public async Task<string> MoveToTrash(int id)
        {
            return await Delete(id);
        }

        /// <summary>
        /// Delete a Coupon Permanently
        /// </summary>
        /// <param name="id">The identifier of coupon to be deleted</param>
        /// <returns></returns>
        public async Task<string> DeletePermanently(int id)
        {
            return await Delete(id, force: true);
        }

        private async Task<string> Delete(int id, bool force = false)
        {
            var apiEndpoint = String.Format("coupons/{0}", id);
            var parameters = new Dictionary<string, string> { { "force", force.ToString().ToLower() } };
            return (await Delete<dynamic>(apiEndpoint, parameters)).message;
        }

        /// <summary>
        /// View Coupon Count
        /// </summary>
        /// <param name="parameters">Parameter to filter list of Coupons</param>
        /// <returns>Number of coupon count</returns>
        public async Task<int> Count(Dictionary<string, string> parameters = null)
        {
            return (await Get<dynamic>(apiEndpoint: "coupons/count", parameters: parameters)).count;
        }
    }
}





