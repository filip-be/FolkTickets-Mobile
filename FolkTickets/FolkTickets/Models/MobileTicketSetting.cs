using System.Linq;

namespace FolkTickets.Models
{
    /// <summary>
    /// BFT ticket setting
    /// </summary>
    public class MobileTicketSetting : MobileTicket
    {
        /// <summary>
        /// Visible on the orders view
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is MobileTicketSetting)
            {
                var otherSetting = obj as MobileTicketSetting;
                if(!ProductID.HasValue || !otherSetting.ProductID.HasValue)
                {
                    return TicketID.Equals(otherSetting.TicketID);
                }
                else
                {
                    return ProductID.Equals(otherSetting.ProductID);
                }
            }

            if(obj is MobileOrder)
            {
                return (obj as MobileOrder).Tickets.Any(t => t.TicketID.Equals(TicketID));
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return TicketID ?? 0;
        }
    }
}
