using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FolkTickets.Controls
{
    public class BooleanToIconConverter : IValueConverter
    {
        private string IconUnknown { get; set; } = "fas-question";
        private string IconTrue { get; set; } = "fas-check";
        private string IconFalse { get; set; } = "fas-times";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return IconUnknown;
            }
            return (bool)value ? IconTrue : IconFalse;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IconTrue.Equals(value))
            {
                return true;
            }
            else if (IconFalse.Equals(value))
            {
                return false;
            }
            else
            {
                return null;
            }
        }
    }
}
