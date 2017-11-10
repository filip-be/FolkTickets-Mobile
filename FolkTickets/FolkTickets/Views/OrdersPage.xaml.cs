using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FolkTickets.ViewModels;
using FolkTickets.Helpers;
//using ZXing.Net.Mobile.Forms;

namespace FolkTickets.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrdersPage : ContentPage
    {
        private OrdersViewModel ViewModel;
        public ObservableCollection<string> Items { get; set; }

        public OrdersPage()
        {
            InitializeComponent();

            Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5"
            };

            BindingContext = this;// ViewModel = new OrdersViewModel();
            MessagingCenter.Subscribe<MessagingCenterAlert>(this, "Error", async (item) =>
            {
                await DisplayAlert(item.Title, item.Message, item.Cancel);
            });
            //MessagingCenter.Subscribe<OrdersViewModel, ZXingScannerPage>(this, "DisplayScanPage", async (view, scanPage) =>
            //{
            //    await Navigation.PushAsync(scanPage);
            //});
        }

        async void Handle_ItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private async void ScanQR_Clicked(object sender)
        {
            try
            {
                //MessagingCenter.Send(this, "ScanQR");
                await Navigation.PopToRootAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", string.Format("Unable to scan QR code: {0}", ex.Message), "OK");
            }
        }
    }
}