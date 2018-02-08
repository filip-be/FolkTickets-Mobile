using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FolkTickets.Controls
{
    public class TicketStatusToIconConverter : IValueConverter
    {
        private string IconUnknown { get; set; } = "fa-question";
        private string IconNew { get; set; } = "fa-sticky-note";
        private string IconChecked { get; set; } = "fa-check-square";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string statusIcon;
            switch (value as int?)
            {
                case 1:
                    statusIcon = IconNew;
                    break;
                case 2:
                    statusIcon = IconChecked;
                    break;
                default:
                    statusIcon = IconUnknown;
                    break;
            };
            return statusIcon;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(IconNew.Equals(value))
            {
                return 1;
            }
            else if(IconChecked.Equals(value))
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
