using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FolkTickets.Models
{
    public class MobileTicket
    {
        /// <summary>
        /// Ticket ID
        /// </summary>
        public int? ID { get; set; }
        /// <summary>
        /// Ticket definition ID
        /// </summary>
        public int? TicketID { get; set; }
        /// <summary>
        /// Ticket Order ID
        /// </summary>
        public int? OrderID { get; set; }
        /// <summary>
        /// Ticket order item ID
        /// </summary>
        public int? OrderItemID { get; set; }
        /// <summary>
        /// Ticket hash
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// Ticket timestamp
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// Ticket status
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// Ticket event ID
        /// </summary>
        public int? EventID { get; set; }
        /// <summary>
        /// Ticket event name
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// Ticket product ID
        /// </summary>
        public int? ProductID { get; set; }
        /// <summary>
        /// Ticket product name
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Ticket product short description
        /// </summary>
        public string ProductShortDescription { get; set; }
        /// <summary>
        /// Status color for UI
        /// </summary>
        public Color StatusColor
        {
            get
            {
                if(Status == 1)
                {
                    return Color.Default;
                }
                else
                {
                    return Color.LightGray;
                }
            }
        }
    }
}
