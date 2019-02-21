using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using FolkTickets.Views;
using FolkTickets.Models;
using ZXing.Net.Mobile.Forms;
using ZXing.Mobile;
using System.Windows.Input;
using FolkTickets.Services;
using FolkTickets.Helpers;
using Plugin.Iconize;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;

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
        /// <summary>
        /// BFT orders list
        /// /// </summary>
        protected IEnumerable<MobileOrder> Orders { get; set; }
        /// <summary>
        /// Tickets settings
        /// </summary>
        private IList<MobileTicketSetting> TicketsSettings { get; set; }
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
        /// Settings command
        /// </summary>
        public ICommand SettingsCommand { get; protected set; }
        /// <summary>
        /// Private variable - search value
        /// </summary>
        private string _SearchText = string.Empty;
        /// <summary>
        /// Orders settings page
        /// </summary>
        private OrdersSettingsPage _OrdersSettingsPage { get; set; }
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
            SettingsCommand = new Command(EditSettings);
            PropertyChanged += OnSearchTextChanged;
            MessagingCenter.Subscribe<OrdersSettingsViewModel>(this, "FilterOrders", (sender) =>
            {
                FilterOrders();
            });
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
                    CompareInfo ci = new CultureInfo("en-US").CompareInfo;
                    CompareOptions co = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;
                    
                    IEnumerable<MobileOrder> limitedOrders = Orders
                        .Where(o =>
                            ci.IndexOf(o.OrderId.ToString(), SearchText, co) != -1
                            || ci.IndexOf(o.CustomerName, SearchText, co) != -1
                            || ci.IndexOf(o.CustomerNote, SearchText, co) != -1
                            || ci.IndexOf(o.CustomerPhone, SearchText, co) != -1
                            || ci.IndexOf(o.CustomerMail, SearchText, co) != -1
                            || ci.IndexOf(o.OrderKey, SearchText, co) != -1
                            || (o.Tickets != null && o.Tickets.Where(t => ci.IndexOf(t.Hash, SearchText, co) != -1).Any())
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

        private void EditSettings()
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

                OrdersSettingsViewModel model = new OrdersSettingsViewModel(TicketsSettings);

                _OrdersSettingsPage = new OrdersSettingsPage(model);
                tabbedPage.Children.Add(_OrdersSettingsPage);
                tabbedPage.SelectedItem = _OrdersSettingsPage;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Could not load orders settings: {0}", ex.Message),
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
                if (TicketsSettings == null)
                {
                    TicketsSettings = new List<MobileTicketSetting>();
                }
                TicketsSettings = TicketsSettings
                    .Concat(
                        Orders
                            .SelectMany(o => o.Tickets)
                            .Where(t => !TicketsSettings.Any(et => et.TicketID == t.TicketID))
                            .Select(t => new MobileTicketSetting { TicketID = t.TicketID, Visible = true })
                            .Distinct()
                        ).ToList();
                FilterOrders();
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
        /// Filter displayed orders
        /// </summary>
        private void FilterOrders()
        {
            Items.Clear();

            // Show only orders with visible tickets
            foreach (var order in Orders
                .Where(o => TicketsSettings.Any(t => t.Visible && t.Equals(o))))
            {
                Items.Add(order);
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
