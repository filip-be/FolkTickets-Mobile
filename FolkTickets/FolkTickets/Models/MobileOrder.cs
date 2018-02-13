using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;
using WooCommerceNET.WooCommerce.v2;
using System.Linq;
using FolkTickets.Helpers;

namespace FolkTickets.Models
{
    public class MobileOrder : ObservableObject
    {
        /// <summary>
        /// Order ID
        /// read-only
        /// </summary>
        public int? OrderId { get; set; }
        /// <summary>
        /// Order key (WC order key / BFT ticket hash)
        /// read-only
        /// </summary>
        public string OrderKey { get; set; }
        /// <summary>
        /// Customer first name
        /// read-only
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Customer note defined within the order
        /// read-only
        /// </summary>
        public string CustomerNote { get; set; }
        /// <summary>
        /// Customer phone number
        /// read-only
        /// </summary>
        public string CustomerPhone { get; set; }
        /// <summary>
        /// Customer email address
        /// read-only
        /// </summary>
        public string CustomerMail { get; set; }
        /// <summary>
        /// Order type - FULL / PARTIAL
        /// read-only
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Order status.
        /// read-only
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Order tickets.
        /// read-write
        /// </summary>
        public List<MobileTicket> Tickets { get; set; }
        /// <summary>
        /// Order notes
        /// read-only
        /// </summary>
        public List<BFTOrderNote> OrderNotes { get; set; }
        /// <summary>
        /// Allow edit already checked tickets
        /// </summary>
        public bool AllowEditAll { get; set; }
        /// <summary>
        /// Status color for UI
        /// </summary>
        public Color StatusColor
        {
            get
            {
                Color color = Color.Default;
                switch(Status?.ToLower())
                {
                    case "on-hold":
                        color = Color.LightPink;
                        break;
                }
                return color;
            }
        }
        /// <summary>
        /// Order status - can be checked
        /// </summary>
        public string StatusChecked
        {
            get
            {
                if(string.IsNullOrEmpty(Status) 
                    || !Status.Equals("completed")
                    || Tickets == null
                    || Tickets.Any(t => t.Status == 1))
                {
                    return Status;
                }
                return "checked-all";
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MobileOrder()
        {
        }

        /// <summary>
        /// Create MobileOrder from bftOrder
        /// </summary>
        /// <param name="bftOrder">BFT order object</param>
        public MobileOrder(BFTOrder bftOrder)
        {
            OrderId = bftOrder.OrderId;
            Status = bftOrder.Status;
            CustomerName = bftOrder.OrderBillingName;
            CustomerMail = bftOrder.OrderBillingEmail;
            CustomerPhone = bftOrder.OrderBillingPhone;
            OrderKey = bftOrder.OrderKey;
            CustomerNote = bftOrder.OrderCustomerNote;
            Type = bftOrder.Type;
            OrderNotes = bftOrder.OrderNotes;
            Tickets = bftOrder.Tickets?.Select(t => new MobileTicket()
            {
                ID = t.ID,
                TicketID = t.TicketID,
                OrderID = t.OrderID,
                OrderItemID = t.OrderItemID,
                Hash = t.Hash,
                Timestamp = t.Timestamp,
                Status = t.Status,
            }).ToList();
        }
    }
}
