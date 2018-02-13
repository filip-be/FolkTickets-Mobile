using FolkTickets.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WooCommerceNET.WooCommerce.v2;
using Xamarin.Forms;

namespace FolkTickets.Models
{
    public class MobileTicket : ObservableObject
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
        /// Ticket status - internal
        /// </summary>
        private int? _Status;
        /// <summary>
        /// Ticket status
        /// </summary>
        public int? Status
        {
            get
            {
                return _Status;
            }
            set
            {
                SetProperty(ref _Status, value);
            }
        }
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
        /// Ticket edited info
        /// </summary>
        public bool Edited { get; set; } = false;
        /// <summary>
        /// Is ticket editable - private member
        /// </summary>
        private bool _isEditable = true;
        /// <summary>
        /// Is ticket editable
        /// </summary>
        public bool IsEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                SetProperty(ref _isEditable, value);
            }
        }
    }
}
