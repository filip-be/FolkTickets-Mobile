using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FolkTickets.Controls
{
    public class TicketStatusToIconColorConverter : IValueConverter
    {
        private Color ColorUnknown { get; set; } = Color.Gray;
        private Color ColorNew { get; set; } = Color.LightSkyBlue;
        private Color ColorChecked { get; set; } = Color.OliveDrab;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color statusColor;
            switch (value as int?)
            {
                case 1:
                    statusColor = ColorNew;
                    break;
                case 2:
                    statusColor = ColorChecked;
                    break;
                default:
                    statusColor = ColorUnknown;
                    break;
            };
            return statusColor;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ColorNew.Equals(value))
            {
                return 1;
            }
            else if (ColorChecked.Equals(value))
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }
    }
}
