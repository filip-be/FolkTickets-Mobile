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
using System.Windows.Input;

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
                //Device.BeginInvokeOnMainThread(async () =>
                //{
                //    await DisplayAlert("Scanned Barcode", result.Text, "OK");
                    
                //});
                await Navigation.PopModalAsync();
                Device.BeginInvokeOnMainThread(() => ViewModel.SearchCommand.Execute(null));
            });
        }

        async void Handle_ItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}