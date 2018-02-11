using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FolkTickets.ViewModels;
using FolkTickets.Helpers;
using ZXing.Net.Mobile.Forms;
using FolkTickets.Models;

namespace FolkTickets.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrdersPage : ContentPage
    {
        private OrdersViewModel ViewModel;
        /// <summary>
        /// QR code scanner running
        /// </summary>
        public bool Scanning { get; set; } = false;

        public OrdersPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new OrdersViewModel();
            
            MessagingCenter.Subscribe<OrdersViewModel, MessagingCenterAlert>(this, "Error", async (sender, item) =>
            {
                await DisplayAlert(item.Title, item.Message, item.Cancel);
            });

            MessagingCenter.Subscribe<OrdersViewModel, ZXingScannerPage>(this, "DisplayScanPage", async (view, scanPage) =>
            {
                Scanning = true;
                await Navigation.PushModalAsync(scanPage);
            });

            MessagingCenter.Subscribe<OrdersViewModel, ZXing.Result>(this, "ScanCompleted", async (view, result) =>
            {
                await Navigation.PopModalAsync();
                Scanning = false;
                Device.BeginInvokeOnMainThread(() => ViewModel.SearchCommand.Execute(true));
            });
        }

        private void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            if (e.Item is MobileOrder)
            {
                ViewModel.DisplayBalFolkOrder(((MobileOrder)e.Item).OrderId?.ToString());
            }

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<LoginViewModel, MessagingCenterAlert>(this, "Error");
        }
    }
}