using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FolkTickets.Controls
{
    /// <summary>
    /// Custom control for WooCommerce order
    /// </summary>
    public class OrderCell : ViewCell
    {
        public int OrderId
        {
            get
            {
                return (int)GetValue(OrderIdProperty);
            }
            set
            {
                SetValue(OrderIdProperty, value);
            }
        }
        public string CustomerFirstName
        {
            get
            {
                return (string)GetValue(CustomerFirstNameProperty);
            }
            set
            {
                SetValue(CustomerFirstNameProperty, value);
            }
        }
        public string CustomerLastName
        {
            get
            {
                return (string)GetValue(CustomerLastNameProperty);
            }
            set
            {
                SetValue(CustomerLastNameProperty, value);
            }
        }
        public string OrderStatus
        {
            get
            {
                return (string)GetValue(OrderStatusProperty);
            }
            set
            {
                SetValue(OrderStatusProperty, value);
            }
        }
        public Color TextColor
        {
            get
            {
                return (Color)GetValue(TextColorProperty);
            }
            set
            {
                SetValue(TextColorProperty, value);
            }
        }
        public Color DetailColor
        {
            get
            {
                return (Color)GetValue(DetailColorProperty);
            }
            set
            {
                SetValue(DetailColorProperty, value);
            }
        }

        public static BindableProperty OrderIdProperty = BindableProperty.Create(
                "OrderId",
                typeof(int),
                typeof(OrderCell),
                0);
        public static BindableProperty CustomerFirstNameProperty = BindableProperty.Create(
                "CustomerFirstName",
                typeof(string),
                typeof(OrderCell));
        public static BindableProperty CustomerLastNameProperty = BindableProperty.Create(
                "CustomerLastName",
                typeof(string),
                typeof(OrderCell));
        public static BindableProperty OrderStatusProperty = BindableProperty.Create(
                "OrderStatus",
                typeof(string),
                typeof(OrderCell));
        public static BindableProperty TextColorProperty = BindableProperty.Create(
                "TextColor",
                typeof(Color),
                typeof(OrderCell),
                Color.Accent);
        public static BindableProperty DetailColorProperty = BindableProperty.Create(
                "DetailColor",
                typeof(Color),
                typeof(OrderCell),
                Color.Default);

        protected Label LabelOrder, LabelCustomer, LabelStatus;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            LabelOrder.Text = $"Order {OrderId}";
            LabelOrder.TextColor = TextColor;
            LabelCustomer.Text = $"{CustomerFirstName} {CustomerLastName}";
            LabelOrder.TextColor = DetailColor;
            LabelStatus.Text = $"Status: {OrderStatus}";
            LabelStatus.TextColor = DetailColor;
            View.BackgroundColor = (Color)new StatusToColorConverter().Convert(OrderStatus, typeof(Color), null, System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Order Item Cell constructor
        /// </summary>
        public OrderCell() : base()
        {
            // Create cell wrapper
            Grid cellWrapper = new Grid
            {
                Padding = new Thickness(5),
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = GridLength.Auto }
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = GridLength.Star },
                    new ColumnDefinition() { Width = GridLength.Auto }
                },
                BackgroundColor =
                    (Color) new StatusToColorConverter().Convert(OrderStatus, typeof(Color), null, System.Globalization.CultureInfo.CurrentCulture),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            // Initialize labels
            LabelOrder = new Label() { Text = $"Order {OrderId}", TextColor = TextColor };
            LabelCustomer = new Label() { Text = $"{CustomerFirstName} {CustomerLastName}", TextColor = DetailColor };
            LabelStatus = new Label() { Text = $"Status: {OrderStatus}", TextColor = DetailColor, HorizontalTextAlignment = TextAlignment.End };

            // Add
            cellWrapper.Children.Add(LabelOrder, 0, 0);
            cellWrapper.Children.Add(LabelCustomer, 0, 1);
            cellWrapper.Children.Add(LabelStatus, 1, 1);

            View = cellWrapper;
        }
    }
}
