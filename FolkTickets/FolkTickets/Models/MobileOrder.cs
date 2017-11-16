using System;
using System.Collections.Generic;
using System.Text;
using WooCommerceNET.WooCommerce.v2;

namespace FolkTickets.Models
{
    public class MobileOrder : BFTOrder
    {
        public Order WCOrder { get; set; }

        public MobileOrder() : base()
        {
        }

        public MobileOrder(BFTOrder order)
        {
            Tickets = order.Tickets;
            Status = order.Status;
            Type = order.Type;
            OrderId = order.OrderId;
        }
    }
}
