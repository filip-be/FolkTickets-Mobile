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
                await Navigation.PushModalAsync(scanPage);
            });

            MessagingCenter.Subscribe<OrdersViewModel, ZXing.Result>(this, "ScanCompleted", async (view, result) =>
            {
                await Navigation.PopModalAsync();
                Device.BeginInvokeOnMainThread(() => ViewModel.SearchCommand.Execute(null));
            });
        }

        private async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            if (e.Item is MobileOrder)
            {
                await ViewModel.DisplayBalFolkOrder(((MobileOrder)e.Item).OrderId?.ToString());
            }

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<LoginViewModel, MessagingCenterAlert>(this, "Error");
            MessagingCenter.Unsubscribe<LoginViewModel, ZXingScannerPage>(this, "DisplayScanPage");
            MessagingCenter.Unsubscribe<LoginViewModel, ZXing.Result>(this, "ScanCompleted");
        }
    }
}