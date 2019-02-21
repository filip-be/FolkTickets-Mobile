using FolkTickets.Helpers;
using FolkTickets.Models;
using FolkTickets.Services;
using Plugin.Iconize;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FolkTickets.ViewModels
{
    /// <summary>
    /// ViewModel for Orders Settings
    /// </summary>
    public class OrdersSettingsViewModel : BaseViewModel
    {
        /// <summary>
        /// Tickets settings
        /// </summary>
        public IList<MobileTicketSetting> TicketsSettings
        {
            get
            {
                return _ProductSettings;
            }
            protected set
            {
                SetProperty(ref _ProductSettings, value);
            }
        }

        private IList<MobileTicketSetting> _ProductSettings;

        /// <summary>
        /// Private member for tickets settings
        /// </summary>
        private IList<MobileTicketSetting> _TicketsSettings { get; set; }

        /// <summary>
        /// Close
        /// </summary>
        public ICommand CloseClicked { get; protected set; }

        /// <summary>
        /// Ticket setting clicked
        /// </summary>
        public ICommand SettingClicked { get; protected set; }

        /// <summary>
        /// Refresh settings
        /// </summary>
        public ICommand RefreshSettings { get; set; }

        public OrdersSettingsViewModel(IList<MobileTicketSetting> ticketsSettings) : base()
        {
            Title = "Settings";

            TicketsSettings = new List<MobileTicketSetting>();
            _TicketsSettings = ticketsSettings;


            CloseClicked = new Command(ClosePage);
            SettingClicked = new Command(PopSetting);
            RefreshSettings = new Command(InitializeTickets);
        }

        /// <summary>
        /// Initialize tickets information
        /// </summary>
        private async void InitializeTickets()
        {
            IsBusy = true;
            try
            {
                // Do not proceed if ticket settings are empty
                if(_TicketsSettings == null)
                {
                    return;
                }

                // Get tickets without product information
                var ticketsToCheck = _TicketsSettings
                    .Where(t => t.TicketID.HasValue && !t.ProductID.HasValue);

                // Loop tickets to check
                while(ticketsToCheck.Any())
                {
                    // Get first
                    var ticket = ticketsToCheck.FirstOrDefault();

                    // Check if there is another ticket with same ticketId
                    var productTicket = _TicketsSettings
                        .FirstOrDefault(t => 
                            t.TicketID.Equals(ticket.TicketID) 
                            && t.ProductID.HasValue);
                    
                    // Update ticket to avoid reading same information multiple times
                    if (productTicket != null)
                    {
                        ticket.ProductID = productTicket.ProductID;
                        ticket.EventID = productTicket.EventID;
                        ticket.EventName = productTicket.EventName;
                        ticket.ProductName = productTicket.ProductName;
                        ticket.ProductShortDescription = productTicket.ProductShortDescription;
                        continue;
                    }

                    // Read ticket info
                    var ticketInfo = await WCService.GetBFTTicket((int)ticket.TicketID);

                    // Update ticket
                    ticket.ProductID = ticketInfo.ProductID;
                    ticket.EventID = ticketInfo.EventID;
                    ticket.EventName = ticketInfo.EventName;
                    ticket.ProductName = ticketInfo.ProductName;
                    ticket.ProductShortDescription = ticketInfo.ProductShortDescription;
                }

                // Refresh settings
                TicketsSettings = _TicketsSettings.Distinct().ToList();
            }
            catch(Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Could not initialize tickets: {0}", ex.Message),
                    Cancel = "OK"
                });
                ClosePage();
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Update ticket visibility
        /// </summary>
        private void PopSetting(object obj)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                if (obj == null || !(obj is MobileTicketSetting))
                {
                    MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                    {
                        Title = "Error",
                        Message = $"Invalid object clicked: {obj}",
                        Cancel = "OK"
                    });
                    return;
                }
                MobileTicketSetting setting = obj as MobileTicketSetting;
                setting.Visible = !setting.Visible;

                foreach(var ticketSetting in _TicketsSettings
                    .Where(s => s.ProductID.Equals(setting.ProductID)))
                {
                    ticketSetting.Visible = setting.Visible;
                }

                // Refresh settings
                TicketsSettings = null;
                TicketsSettings = _TicketsSettings.Distinct().ToList();

                // Filter orders
                MessagingCenter.Send(this, "FilterOrders");
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = $"Error occured: {ex.Message}",
                    Cancel = "OK"
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Close current page
        /// </summary>
        private void ClosePage()
        {
            if (IsBusy)
                return;

            IsBusy = true;
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

                Page view = tabbedPage.Children.Where(p => p.BindingContext == this).FirstOrDefault();
                if (view != null)
                {
                    tabbedPage.Children.Remove(view);
                }
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Could not close page: {0}", ex.Message),
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
