using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;
using WooCommerceNET.WooCommerce.v2;

namespace FolkTickets.Models
{
    public class MobileOrder
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
    }
}
