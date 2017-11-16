using FolkTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v2;
using Xamarin.Auth;
using Xamarin.Forms;

namespace FolkTickets.Services
{
    /// <summary>
    /// WooCommerce service
    /// </summary>
    public class WCService
    {
        /// <summary>
        /// Test saved APP credentials
        /// </summary>
        /// <param name="throwErrors">Throw errors if something is wrong</param>
        /// <param name="userAccount">User account object</param>
        /// <returns>True if the connection was successfull</returns>
        public static async Task<bool> TestCredentialsAsync(bool throwErrors, Account userAccount = null)
        {
            try
            {
                WCObject wcObj = GetWCApiObject(userAccount);
                if (wcObj == null)
                {
                    throw new Exception("Could not initialize WC object");
                }

                await wcObj.SystemStatus.GetAll();

                return true;
            }
            catch (Exception)
            {
                if (throwErrors)
                {
                    throw;
                }
                return false;
            }
        }

        /// <summary>
        /// Get WC REST API object
        /// </summary>
        /// <param name="userAccount">user account object</param>
        /// <returns>REST API object or null</returns>
        private static WCObject GetWCApiObject(Account userAccount)
        {
            if (userAccount == null)
            {
                userAccount = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
            }

            if (userAccount == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(userAccount.Username))
            {
                throw new ArgumentException("Page URI cannot be empty");
            }
            if (!userAccount.Properties.ContainsKey("ApiKey") || string.IsNullOrWhiteSpace(userAccount.Properties["ApiKey"]))
            {
                throw new ArgumentException("API Key cannot be empty");
            }
            if (!userAccount.Properties.ContainsKey("ApiSecret") || string.IsNullOrWhiteSpace(userAccount.Properties["ApiSecret"]))
            {
                throw new ArgumentException("API Secret cannot be empty");
            }
            if (!userAccount.Properties.ContainsKey("Lang") || string.IsNullOrWhiteSpace(userAccount.Properties["ApiSecret"]))
            {
                throw new ArgumentException("Language cannot be empty");
            }

            RestAPI rest = new RestAPI(userAccount.Username,
                userAccount.Properties["ApiKey"],
                userAccount.Properties["ApiSecret"],
                true,
                null,
                null,
                null,
                null,
                new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Accept-Language", userAccount.Properties["Lang"]) });
            if (rest == null)
            {
                throw new Exception("Could initialize RestAPI object");
            }

            return new WCObject(rest);
        }

        /// <summary>
        /// Get all WooCommerce orders
        /// </summary>
        public static async Task<IEnumerable<MobileOrder>> GetAllWCOrders()
        {
            try
            {
                WCObject api = GetWCApiObject(null);
                List<Order> orders = await api.Order.GetAll();
                return orders.Select(o => new MobileOrder() { OrderId = o.id, Status = o.status });
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not get all WC orders: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Get BFT orders with tickets
        /// </summary>
        /// <param name="key">Order ID / Order Key / Ticket hash</param>
        public static async Task<MobileOrder> GetBFTOrder(string key)
        {
            try
            {
                WCObject api = GetWCApiObject(null);
                BFTOrder bftOrder = await api.BFTOrder.Get(key);
                MobileOrder order = new MobileOrder(bftOrder);
                order.WCOrder = await api.Order.Get(order.OrderId.ToString());

                return order;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
