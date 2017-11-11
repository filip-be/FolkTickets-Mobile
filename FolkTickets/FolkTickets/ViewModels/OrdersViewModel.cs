using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using FolkTickets.Views;
using FolkTickets.Helpers;
using ZXing.Net.Mobile.Forms;
using ZXing.Mobile;
using System.Windows.Input;

namespace FolkTickets.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        public ObservableCollection<string> Items { get; protected set; }
        public ICommand ScanClicked { get; protected set; }
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

            ScanClicked = new Command(ScanQR);
        }

        private void ScanQR()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = false,
                    //TryHarder = true,
                    //PossibleFormats = new List<ZXing.BarcodeFormat>
                    //{
                    //    ZXing.BarcodeFormat.QR_CODE
                    //}
                };

                ZXingScannerPage scanPage = new ZXingScannerPage(options)
                {
                    DefaultOverlayBottomText = "Scan ticket QR code"
                };

                scanPage.OnScanResult += (result) =>
                {
                    // Stop scanning
                    //scanPage.IsScanning = false;
                    scanPage.SendBackButtonPressed();

                    MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                    {
                        Title = "Error",
                        Message = string.Format("Scanned text: {0}", result.Text),
                        Cancel = "OK"
                    });
                    // Pop the page and show the result
                    //Device.BeginInvokeOnMainThread(() => {
                    //    Navigation.PopAsync();
                    //    DisplayAlert("Scanned Barcode", result.Text, "OK");
                    //});
                };

                MessagingCenter.Send(this, "DisplayScanPage", scanPage);
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Unable to scan QR: {0}", ex.Message),
                    Cancel = "OK"
                });
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
