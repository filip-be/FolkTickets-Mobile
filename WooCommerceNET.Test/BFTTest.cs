using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Collections.Generic;
using WooCommerceNET.WooCommerce.v2;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Globalization;

namespace WooCommerceNET.Test
{
    [TestClass]
    public class BFTTest
    {
        [TestMethod]
        public void TestGetOrder()
        {
            string APIUri = ConfigurationManager.AppSettings["WCApiUri"];
            Assert.IsNotNull(APIUri);

            string ApiKey = ConfigurationManager.AppSettings["WCApiKey"];
            Assert.IsNotNull(ApiKey);

            string ApiSecret = ConfigurationManager.AppSettings["WCApiSecret"];
            Assert.IsNotNull(ApiSecret);

            string ApiLanguage = ConfigurationManager.AppSettings["WCApiLanguage"];
            Assert.IsNotNull(ApiLanguage);

            RestAPI rest = new RestAPI(APIUri,
                    ApiKey,
                    ApiSecret,
                    true,
                    null,
                    null,
                    null,
                    null,
                    new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Accept-Language", ApiLanguage) });
            Assert.IsNotNull(rest);

            WCObject wc = new WCObject(rest);
            Assert.IsNotNull(wc);

            Trace.WriteLine("Getting WC orders");
            List<Order> orders = wc.Order.GetAll().Result;
            Assert.IsNotNull(orders);
            Assert.IsTrue(orders.Count > 0, "WC Endpoint doesn't contain any orders!");
            Trace.WriteLine(string.Format("Found {0} orders", orders.Count));

            Order lastOrder = orders.OrderBy(w => w.date_created).Last();
            Assert.IsNotNull(lastOrder.id);
            Assert.IsNotNull(lastOrder.order_key);
            Trace.WriteLine(string.Format("Using last order. ID: {0}, Key: {1}",
                lastOrder.id, lastOrder.order_key));

            Trace.WriteLine("Looking for BFT order");
            BFTOrder bftOrder = wc.BFTOrder.Get(lastOrder.id.ToString()).Result;
            Assert.IsNotNull(bftOrder);
            Assert.AreEqual(lastOrder.id, bftOrder.OrderId);
            Assert.AreEqual(lastOrder.status, bftOrder.Status);
            Assert.AreEqual("Full", bftOrder.Type);

            Assert.IsNotNull(bftOrder.Tickets);
            Assert.IsTrue(bftOrder.Tickets.Count() > 0, "Order has no tickets assigned");

            Trace.WriteLine(string.Format("Found BFT order. ID: {0}, Status: {1}, Type: {2}, Tickets: {3}",
                bftOrder.OrderId, 
                bftOrder.Status,
                bftOrder.Type,
                bftOrder.Tickets.Count()));
            Trace.WriteLine("Validating tickets");
            foreach (var ticket in bftOrder.Tickets)
            {
                Trace.WriteLine(string.Format("Validating ticket. ID: {0}, TicketID: {1}, OrderID: {2}, OrderItemId: {3}, Hash: {4}, Timestamp: {5}, Status: {6}",
                    ticket.ID,
                    ticket.TicketID,
                    ticket.OrderID,
                    ticket.OrderItemID,
                    ticket.Hash,
                    ticket.Timestamp,
                    ticket.Status));
                Assert.IsNotNull(ticket.ID);
                Assert.IsNotNull(ticket.TicketID);
                Assert.IsNotNull(ticket.OrderID);
                Assert.AreEqual(lastOrder.id, ticket.OrderID);
                Assert.IsNotNull(ticket.OrderItemID);
                Assert.IsNotNull(ticket.Hash);
                Assert.IsNotNull(ticket.Timestamp);
                Assert.IsNotNull(ticket.Status);
            }

            Trace.WriteLine(string.Format("Getting BFT order by key: {0}", lastOrder.order_key));
            BFTOrder bftOrder2 = wc.BFTOrder.Get(lastOrder.order_key).Result;
            Assert.IsNotNull(bftOrder2);
            Assert.AreEqual(bftOrder, bftOrder2);
            Trace.Write("Completed, result was equal!");
        }

        [TestMethod]
        public void CompareStrings()
        {
            CompareInfo ci = new CultureInfo("en-US").CompareInfo;
            CompareOptions co = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;
            string source = "Zażółć Michała jaźń";
            string search = "michal";

            int position = ci.IndexOf(source, search, co);
            Assert.IsTrue(position >= 0, $"Could not find '{search}' in '{source}'");

        }
    }
}
