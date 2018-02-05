using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FolkTickets.Controls
{
    public class StatusToColorConverter : IValueConverter
    {
        public static Color BackgroundgColorDefault { get; set; } = Color.FromHex("#FAFAFA");
        public static Color BackgroundColorFailed { get; set; } = Color.LightPink;
        public static Color BackgroundColorOnHold { get; set; } = Color.FromHex("#C0C0C0");
        public static Color BackgroundColorUndefined { get; set; } = Color.LightPink;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color bgColor = BackgroundgColorDefault;
            switch (value as string)
            {
                case "pending":
                case "processing":
                case "onhold":
                case "on-hold":
                    bgColor = BackgroundColorOnHold;
                    break;
                case "completed":
                    bgColor = BackgroundgColorDefault;
                    break;
                case "failed":
                case "cancelled":
                case "refunded":
                    bgColor = BackgroundColorFailed;
                    break;
                default:
                    bgColor = BackgroundColorUndefined;
                    break;
            };
            return bgColor;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "unknown";
        }
    }
}
