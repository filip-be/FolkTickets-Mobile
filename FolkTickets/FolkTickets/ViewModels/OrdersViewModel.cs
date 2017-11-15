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
using WooCommerceNET.WooCommerce.v2;
using FolkTickets.Services;

namespace FolkTickets.ViewModels
{
    /// <summary>
    /// ViewModel for Orders
    /// </summary>
    public class OrdersViewModel : BaseViewModel
    {
        /// <summary>
        /// List of itmes
        /// </summary>
        public ObservableCollection<BFTOrder> Items { get; protected set; }
        /// <summary>
        /// QR scan button clicked command
        /// </summary>
        public ICommand ScanClicked { get; protected set; }
        /// <summary>
        /// Load all WC orders button click command
        /// </summary>
        public ICommand LoadAllOrdersCommand { get; protected set; }
        /// <summary>
        /// Search order command
        /// </summary>
        public ICommand SearchCommand { get; protected set; }
        /// <summary>
        /// Private variable - search value
        /// </summary>
        private string _SearchText = string.Empty;
        /// <summary>
        /// Search value
        /// </summary>
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
        /// <summary>
        /// Default constructor
        /// </summary>
        public OrdersViewModel() : base()
        {
            Title = "Orders";

            Items = new ObservableCollection<BFTOrder>();

            ScanClicked = new Command(ScanQR);
            SearchCommand = new Command(FindOrder);
            LoadAllOrdersCommand = new Command(LoadAllOrders);
        }

        /// <summary>
        /// Load all WC orders
        /// </summary>
        private async void LoadAllOrders()
        {
            try
            {
                IEnumerable<BFTOrder> orders = await WCService.GetAllWCOrders();
                Items.Clear();
                foreach (var order in orders)
                {
                    Items.Add(order);
                }
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Unable to get WC orders: {0}", ex.Message),
                    Cancel = "OK"
                });
            }
        }

        /// <summary>
        /// Scan QR code action
        /// </summary>
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
                    MessagingCenter.Send(this, "ScanCompleted", result);
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

        /// <summary>
        /// Find specific order
        /// </summary>
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
