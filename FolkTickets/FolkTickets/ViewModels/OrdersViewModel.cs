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
        public ICommand SearchCommand { get; protected set; }
        private string _SearchText = string.Empty;
        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                SetProperty(ref _SearchText, value);
            }
        }
        public OrdersViewModel() : base()
        {
            Title = "Orders";

            Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5"
            };

            ScanClicked = new Command(ScanQR);
            SearchCommand = new Command(FindOrder);
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
                };

                ZXingScannerPage scanPage = new ZXingScannerPage(options)
                {
                    DefaultOverlayBottomText = "Scan ticket QR code"
                };
                
                scanPage.OnScanResult += (result) =>
                {
                    SearchText = result?.Text;
                    MessagingCenter.Send(this, "ScanCompleted", result); ;
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

        private void FindOrder()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Order to search: {0}", SearchText),
                    Cancel = "OK"
                });
            }
            catch(Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Something went wrong: {0}", ex.Message),
                    Cancel = "OK"
                });
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
