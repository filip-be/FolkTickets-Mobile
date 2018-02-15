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
                return orders.Select(o => new MobileOrder(o));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not get all WC orders: {0}", ex.Message), ex);
            }
        }

        public static async Task<IEnumerable<Statistic>> GetStatistics()
        {
            try
            {
                // Initialize connection object
                WCObject api = GetWCApiObject(null);

                // Return statistics
                var statistics = await api.BFTStatistic.GetAll();

                // Copy array
                return statistics.Select(s => new Statistic(s));
            }
            catch (Exception)
            {
                throw;
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
                MobileOrder order = new MobileOrder(bftOrder);
                
                foreach(var ticket in order.Tickets)
                {
                    BFTTicket bftTicket = tickets.Where(t => t.Id == ticket.TicketID).FirstOrDefault();
                    if(bftTicket == null)
                    {
                        bftTicket = await api.BFTTicket.Get((int)ticket.TicketID);
                    }

                    ticket.ProductID = bftTicket.ProductID;
                    ticket.EventID = bftTicket.EventID;

                    BFTEvent bftEvent = events.Where(e => e.Id == ticket.EventID).FirstOrDefault();
                    if (bftEvent == null)
                    {
                        bftEvent = await api.BFTEvent.Get((int)ticket.EventID);
                    }
                    ticket.EventName = bftEvent.Name;

                    Product product = products.Where(p => p.id == ticket.ProductID).FirstOrDefault();
                    if (product == null)
                    {
                        product = await api.Product.Get((int)ticket.ProductID);
                    }
                    ticket.ProductName = product.name;
                    ticket.ProductShortDescription = product.short_description;
                }
                
                return order;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Add order note
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="note">Note string</param>
        /// <returns>bool</returns>
        public static async Task<bool> AddOrderNote(int orderId, string note)
        {
            string result = string.Empty;

            OrderNote orderNote = new OrderNote
            {
                customer_note = false,
                note = note,
            };

            WCObject api = GetWCApiObject(null);

            OrderNote returnNote = await api.Order.Notes.Add(orderNote, orderId);
            if(returnNote != null && note.Equals(returnNote.note))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update BFT order
        /// </summary>
        /// <param name="order">Order object</param>
        /// <returns>error message or empty string if success</returns>
        public static async Task<string> UpdateOrder(MobileOrder order)
        {
            string error = string.Empty;

            try
            {
                if(order?.Tickets == null)
                {
                    return "Order doesn't contain any tickets";
                }

                List<MobileTicket> tickets = order.Tickets.ToArray().ToList();
                tickets.RemoveAll(t => !t.Edited);
                
                if (!tickets.Any())
                {
                    return $"No tickets has been modified";
                }
                
                BFTOrder tempOrder = new BFTOrder
                {
                    OrderId = order.OrderId,
                    Tickets = tickets.Select(t => new BFTOrderTicket
                    {
                        ID = t.ID,
                        OrderID = t.OrderID,
                        OrderItemID = t.OrderItemID,
                        TicketID = t.TicketID,
                        Hash = t.Hash,
                        Status = t.Status,
                        Timestamp = t.Timestamp
                    }).ToList()
                };
                
                WCObject api = GetWCApiObject(null);
                BFTOrder updatedOrder = await api.BFTOrder.Update((int)tempOrder.OrderId, tempOrder);
                if(updatedOrder == null)
                {
                    return "Could not update order - return message is null";
                }
                if(!string.IsNullOrEmpty(updatedOrder.OrderCustomerNote))
                {
                    return $"Error updating order: {updatedOrder.OrderCustomerNote}";
                }
            }
            catch (Exception ex)
            {
                error = $"Error updating order: {ex.Message}";
            }

            return error;
        }
    }
}
