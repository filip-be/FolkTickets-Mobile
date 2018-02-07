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
            bool useSSL = false;
            if(userAccount.Properties.ContainsKey("UseSSL"))
            {
                bool.TryParse(userAccount.Properties["UseSSL"], out useSSL);
            }

            // Determine connection protocol
            string connectionProtocol = useSSL ? "https" : "http";
            // Prepare connection string
            string wcAddress = $"{connectionProtocol}://{userAccount.Username}/wp-json/wc/v2";

            // Create RestAPI object using prepared connection string
            RestAPI rest = new RestAPI(wcAddress,
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
        /*
        private static IEnumerable<MobileOrder> FakeOrders = new List<MobileOrder>()
                {
                    new MobileOrder()
                    {
                        OrderId = 1,
                        OrderKey = "123",
                        Status = "on-hold",
                        Type = "Full",
                        CustomerFirstName = "John",
                        CustomerLastName = "Smith",
                        Tickets = new List<MobileTicket>()
                        {
                            new MobileTicket()
                            {
                                ID = 1,
                                OrderID = 1,
                                EventID = 1,
                                EventName = "TestEvent",
                                Hash = "H1",
                                OrderItemID = 1,
                                ProductID = 1,
                                ProductName = "Test Product",
                                ProductShortDescription = "Short description 1",
                                Status = 1,
                                TicketID = 1,
                                Timestamp = "NOW"
                            },
                            new MobileTicket()
                            {
                                ID = 2,
                                OrderID = 1,
                                EventID = 1,
                                EventName = "TestEvent",
                                Hash = "H2",
                                OrderItemID = 1,
                                ProductID = 1,
                                ProductName = "Test Product",
                                ProductShortDescription = "Short description 1",
                                Status = 10,
                                TicketID = 1,
                                Timestamp = "NOW2"
                            },
                        }
                    }
                };
        */
        /// <summary>
        /// Get all WooCommerce orders
        /// </summary>
        public static async Task<IEnumerable<MobileOrder>> GetAllWCOrders()
        {
            try
            {
                //return FakeOrders;
                
                // Initialize connection object
                WCObject api = GetWCApiObject(null);

                List<BFTOrder> orders = await api.BFTOrder.GetAll();

                // Return orders
                return orders.Select(o => new MobileOrder()
                {
                    OrderId = o.OrderId,
                    Status = o.Status,
                    CustomerName = o.OrderBillingName,
                    CustomerMail = o.OrderBillingEmail,
                    CustomerPhone = o.OrderBillingPhone,
                    OrderKey = o.OrderKey,
                    CustomerNote = o.OrderCustomerNote,
                });
                
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
                //MobileOrder order = FakeOrders.FirstOrDefault();
                
                IList<BFTTicket> tickets = new List<BFTTicket>();
                IList<BFTEvent> events = new List<BFTEvent>();
                IList<Product> products = new List<Product>();

                WCObject api = GetWCApiObject(null);
                BFTOrder bftOrder = await api.BFTOrder.Get(key);
                MobileOrder order = new MobileOrder
                {
                    OrderId = bftOrder.OrderId,
                    OrderKey = key,
                    Status = bftOrder.Status,
                    Type = bftOrder.Type
                };

                Order wcOrder = await api.Order.Get(order.OrderId.ToString());
                order.CustomerName = wcOrder.billing.first_name;
                order.CustomerMail = wcOrder.billing.email;
                order.CustomerPhone = wcOrder.billing.phone;
                order.CustomerNote = wcOrder.customer_note;

                order.Tickets = new List<MobileTicket>();
                foreach(var ticket in bftOrder.Tickets)
                {
                    MobileTicket mTicket = new MobileTicket
                    {
                        ID = ticket.ID,
                        TicketID = ticket.TicketID,
                        OrderID = ticket.OrderID,
                        OrderItemID = ticket.OrderItemID,
                        Hash = ticket.Hash,
                        Timestamp = ticket.Timestamp,
                        Status = ticket.Status
                    };

                    BFTTicket bftTicket = tickets.Where(t => t.Id == mTicket.TicketID).FirstOrDefault();
                    if(bftTicket == null)
                    {
                        bftTicket = await api.BFTTicket.Get((int)mTicket.TicketID);
                    }

                    mTicket.ProductID = bftTicket.ProductID;
                    mTicket.EventID = bftTicket.EventID;

                    BFTEvent bftEvent = events.Where(e => e.Id == mTicket.EventID).FirstOrDefault();
                    if (bftEvent == null)
                    {
                        bftEvent = await api.BFTEvent.Get((int)mTicket.EventID);
                    }
                    mTicket.EventName = bftEvent.Name;

                    Product product = products.Where(p => p.id == mTicket.ProductID).FirstOrDefault();
                    if (product == null)
                    {
                        product = await api.Product.Get((int)mTicket.ProductID);
                    }
                    mTicket.ProductName = product.name;
                    mTicket.ProductShortDescription = product.short_description;

                    order.Tickets.Add(mTicket);
                }
                
                return order;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
