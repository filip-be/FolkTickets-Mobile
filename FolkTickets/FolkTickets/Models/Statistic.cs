using System;
using System.Collections.Generic;
using System.Text;
using WooCommerceNET.WooCommerce.v2;

namespace FolkTickets.Models
{
    public class Statistic
    {
        /// <summary>
        /// Event name
        /// read-only
        /// </summary>
        public string Event { get; set; }
        /// <summary>
        /// Product name
        /// read-only
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// Already checked tickets count
        /// read-only
        /// </summary>
        public int? CheckedTickets { get; set; }
        /// <summary>
        /// All tickets count
        /// read-only
        /// </summary>
        public int? TicketsCount { get; set; }

        public Statistic(BFTStatistic s)
        {
            if(s == null)
            {
                return;
            }
            Event = s.Event;
            Product = s.Product;
            CheckedTickets = s.CheckedTickets;
            TicketsCount = s.TicketsCount;
        }
    }
}
