using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using FolkTickets.Views;
using FolkTickets.Helpers;
//using ZXing.Net.Mobile.Forms;

namespace FolkTickets.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        public ObservableCollection<string> Items { get; set; }
        public OrdersViewModel()
        {
            Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5"
            };

            MessagingCenter.Subscribe<OrdersPage>(this, "ScanQR", ScanQRAsync);
        }

        private void ScanQRAsync(OrdersPage page)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                //var scanPage = new ZXingScannerPage();

                //scanPage.OnScanResult += (result) =>
                //{
                //    // Stop scanning
                //    scanPage.IsScanning = false;

                //    MessagingCenter.Send(new MessagingCenterAlert
                //    {
                //        Title = "Error",
                //        Message = string.Format("Scanned text: {0}", result.Text),
                //        Cancel = "OK"
                //    }, "Error");
                //    // Pop the page and show the result
                //    //Device.BeginInvokeOnMainThread(() => {
                //    //    Navigation.PopAsync();
                //    //    DisplayAlert("Scanned Barcode", result.Text, "OK");
                //    //});
                //};

                //MessagingCenter.Send(this, "DisplayScanPage", scanPage);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Unable to scan QR: {0}", ex.Message),
                    Cancel = "OK"
                }, "Error");
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
