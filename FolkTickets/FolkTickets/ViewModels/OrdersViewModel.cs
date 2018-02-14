using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using FolkTickets.Views;
using FolkTickets.Helpers;
using ZXing.Net.Mobile.Forms;
using ZXing.Mobile;
using System.Windows.Input;
using FolkTickets.Services;
using FolkTickets.Models;
using FormsPlugin.Iconize;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FolkTickets.ViewModels
{
    /// <summary>
    /// ViewModel for Orders
    /// </summary>
    public class OrdersViewModel : BaseViewModel
    {
        /// <summary>
        /// List of items
        /// </summary>
        public ObservableCollection<MobileOrder> Items { get; protected set; }
        // BFT orders list
        protected IEnumerable<MobileOrder> Orders { get; set; }
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
        /// Show statistics command
        /// </summary>
        public ICommand ShowStatsCommand { get; protected set; }
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
        /// Private variable - LoadAllOrders button text
        /// </summary>
        private string _LoadAllOrdersText = "Load all orders";
        /// <summary>
        /// LoadAllOrders button text
        /// </summary>
        public string LoadAllOrdersText
        {
            get
            {
                return _LoadAllOrdersText;
            }
            set
            {
                SetProperty(ref _LoadAllOrdersText, value);
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public OrdersViewModel() : base()
        {
            Title = "Orders";

            Items = new ObservableCollection<MobileOrder>();

            ScanClicked = new Command(ScanQR);
            SearchCommand = new Command(FindOrder);
            LoadAllOrdersCommand = new Command(LoadAllOrders);
            ShowStatsCommand = new Command(ShowStats);
            PropertyChanged += OnSearchTextChanged;
        }

        private void OnSearchTextChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("SearchText"))
            {
                try
                {
                    if(Orders == null)
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(SearchText))
                    {
                        Items.Clear();
                        foreach (var order in Orders)
                        {
                            Items.Add(order);
                        }
                        return;
                    }
                    IEnumerable<MobileOrder> limitedOrders = Orders
                        .Where(o =>
                            o.OrderId.ToString().StartsWith(SearchText)
                            || o.CustomerName.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) != -1
                            || o.CustomerNote.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) != -1
                            || o.CustomerPhone.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) != -1
                            || o.CustomerMail.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) != -1
                            || o.OrderKey.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) != -1
                            || (o.Tickets != null && o.Tickets.Where(t => t.Hash.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) != -1).Any())
                            );

                    Items.Clear();
                    foreach (var order in limitedOrders)
                    {
                        Items.Add(order);
                    }
                }
                catch (Exception ex)
                {
                    MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                    {
                        Title = "Error",
                        Message = string.Format("Error filtering orders: {0}", ex.Message),
                        Cancel = "OK"
                    });
                }
            }
        }

        private void ShowStats()
        {
            try
            {
                if (!(App.Current.MainPage is IconNavigationPage)
                        || !((App.Current.MainPage as IconNavigationPage)?.CurrentPage is IconTabbedPage))
                {
                    MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                    {
                        Title = "Error",
                        Message = "Logged succesfully, but there are invalid application pages. Cannot proceed!",
                        Cancel = "OK"
                    });
                    return;
                }
                IconTabbedPage tabbedPage = (App.Current.MainPage as IconNavigationPage).CurrentPage as IconTabbedPage;
                
                var newPage = new StatsPage();
                tabbedPage.Children.Add(newPage);
                tabbedPage.SelectedItem = newPage;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Could not load Bal Folk stats: {0}", ex.Message),
                    Cancel = "OK"
                });
            }
        }

        /// <summary>
        /// Load all WC orders
        /// </summary>
        private async void LoadAllOrders()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                Orders = await WCService.GetAllWCOrders();
                Items.Clear();
                foreach (var order in Orders)
                {
                    Items.Add(order);
                }
                LoadAllOrdersText = "Reload orders";
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
            finally
            {
                IsBusy = false;
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
        private void FindOrder(object param)
        {
            bool force = false;
            if (param is bool)
            {
                force = (bool)param;
            }
            if (IsBusy && !force)
                return;

            IsBusy = true;
            try
            {
                DisplayBalFolkOrder(SearchText);
            }
            catch (Exception ex)
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

        public void DisplayBalFolkOrder(string key)
        {
            try
            {
                if (!(App.Current.MainPage is IconNavigationPage)
                        || !((App.Current.MainPage as IconNavigationPage)?.CurrentPage is IconTabbedPage))
                {
                    MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                    {
                        Title = "Error",
                        Message = "Logged succesfully, but there are invalid application pages. Cannot proceed!",
                        Cancel = "OK"
                    });
                    return;
                }
                IconTabbedPage tabbedPage = (App.Current.MainPage as IconNavigationPage).CurrentPage as IconTabbedPage;

                OrderViewModel model = new OrderViewModel(key);

                var newPage = new OrderPage(model);
                tabbedPage.Children.Add(newPage);
                tabbedPage.SelectedItem = newPage;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Could not load Bal Folk order: {0}", ex.Message),
                    Cancel = "OK"
                });
            }
        }
    }
}
