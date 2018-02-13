using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FolkTickets.Controls
{
    public class TicketEditableBGColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (value is bool) && (bool)value)
            {
                return Color.Default;
            }
            return Color.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Color) && ((Color)value) == Color.Default;
        }
    }
}
