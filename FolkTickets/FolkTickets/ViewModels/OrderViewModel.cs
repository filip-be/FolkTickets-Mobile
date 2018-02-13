using FolkTickets.Helpers;
using FolkTickets.Models;
using FolkTickets.Services;
using FormsPlugin.Iconize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FolkTickets.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        private MobileOrder _Order;
        public MobileOrder Order
        {
            get
            {
                return _Order;
            }
            protected set
            {
                SetProperty(ref _Order, value);
            }
        }
        private bool _partialOrder = true;
        public bool PartialOrder
        {
            get
            {
                return _partialOrder;
            }
            set
            {
                SetProperty(ref _partialOrder, value);
            }
        }

        public ICommand CloseClicked { get; protected set; }
        public ICommand UpdateClicked { get; protected set; }
        public ICommand InfoClicked { get; protected set; }
        public ICommand TicketClicked { get; protected set; }
        public ICommand AddNoteClicked { get; protected set; }
        public ICommand AllowEditAllClicked { get; protected set; }
        public ICommand LoadFullOrderClicked { get; protected set; }
        public ICommand RefreshClicked { get; protected set; }

        public OrderViewModel(string key) : base()
        {
            Title = "Order";

            CloseClicked = new Command(ClosePage);
            UpdateClicked = new Command(UpdateTickets);
            InfoClicked = new Command(ShowInfo);
            TicketClicked = new Command(CheckTicket);
            AddNoteClicked = new Command(AddNote);
            AllowEditAllClicked = new Command(AllowEditAll);
            LoadFullOrderClicked = new Command(LoadFullOrder);
            RefreshClicked = new Command(Refresh);

            Order = new MobileOrder();
            Initialize(key);
        }

        private async void Refresh(object obj)
        {
            if (Order?.OrderId == null)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = "Order ID is empty",
                    Cancel = "OK"
                });
                return;
            }
            await Initialize(Order.OrderKey);
        }

        private async void LoadFullOrder(object obj)
        {
            if (Order?.OrderId == null)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = "Order ID is empty",
                    Cancel = "OK"
                });
                return;
            }
            await Initialize(Order.OrderId.ToString());
        }

        private void AllowEditAll(object obj)
        {
            if (Order?.OrderId == null)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = "Order ID is empty",
                    Cancel = "OK"
                });
                return;
            }
            
            Order.AllowEditAll = !Order.AllowEditAll;

            if (Order.Tickets != null)
            {
                if (Order.AllowEditAll == true)
                {
                    Order.Tickets.ForEach(t => t.IsEditable = true);
                }
                else
                {
                    Order.Tickets.ForEach(t => 
                        {
                            if (!t.Edited
                                && t.Status == 2)
                            {
                                t.IsEditable = false;
                            }
                        });
                }
            }
        }

        private async void AddNote(object obj)
        {
            if (Order?.OrderId == null)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = "Order ID is empty",
                    Cancel = "OK"
                });
                return;
            }

            if (string.IsNullOrEmpty(obj as string))
            {
                // Do not add note twice
                if (IsBusy)
                {
                    return;
                }
                MessagingCenter.Send(this, "Note", new MessagingCenterAlert());
                return;
            }

            IsBusy = true;
            try
            {
                // Add note
                bool res = await WCService.AddOrderNote((int)Order.OrderId, obj as string);

                // Update notes if add has succeeded
                if (res)
                {
                    MobileOrder order = await WCService.GetBFTOrder(Order.OrderKey);
                    Order.OrderNotes = order.OrderNotes;
                }

                // Return result message
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = res ? "Note" : "Error",
                    Message = res ? "Added successfully" : "Error adding note",
                    Cancel = "OK",
                });
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = $"Error adding note: {ex.Message}",
                    Cancel = "OK",
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CheckTicket(object obj)
        {
            try
            {
                if (obj == null || !(obj is MobileTicket))
                {
                    MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                    {
                        Title = "Error",
                        Message = $"Invalid object clicked: {obj}",
                        Cancel = "OK"
                    });
                    return;
                }
                MobileTicket ticket = obj as MobileTicket;
                if (!ticket.IsEditable)
                {
                    return;
                }
                ticket.Edited = true;
                switch (ticket.Status)
                {
                    case 1:
                        ticket.Status = 2;
                        break;
                    case 2:
                        ticket.Status = 1;
                        break;
                    default:
                        MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                        {
                            Title = "Error",
                            Message = $"Invalid ticket status: {ticket.Status}",
                            Cancel = "OK"
                        });
                        return;
                }
                MobileOrder tempOrder = _Order;
                Order = null;
                Order = tempOrder;
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
        }

        private void ShowInfo(object obj)
        {
            if (Order == null)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = "Order is null",
                    Cancel = "OK"
                });
                return;
            }

            string orderDetails = string.Format("{1}{0}{2}{0}{3}{0}{0}Order messages:{0}{4}",
                Environment.NewLine,
                Order.CustomerName,
                Order.CustomerMail,
                Order.CustomerPhone,
                string.Join(Environment.NewLine, Order.OrderNotes.Select(n => $"{n.date_created.date.TrimEnd(new char[] { '0' })} ({n.added_by}):{Environment.NewLine}{n.content}")));

            MessagingCenter.Send(this, "Error", new MessagingCenterAlert
            {
                Title = "Info",
                Message = orderDetails,
                Cancel = "OK"
            });
        }

        private async Task Initialize(string key)
        {
            IsBusy = true;
            try
            {
                Order = await WCService.GetBFTOrder(key);
                PartialOrder = "Partial".Equals(Order?.Type, StringComparison.OrdinalIgnoreCase);
                if(Order?.Tickets != null)
                {
                    Order.Tickets.ForEach(t =>
                    {
                        t.IsEditable = t.Status == 1;
                    });
                }
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Could not load order: {0}", ex.Message),
                    Cancel = "OK"
                });
                ClosePage();
            }
            finally
            {
                IsBusy = false;
            }
        }

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

        private async void UpdateTickets()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                string error = await WCService.UpdateOrder(Order);
                bool success = string.IsNullOrWhiteSpace(error);
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = success ? "Success" : "Error",
                    Message = success ? "Order has been updated successfully" : $"Update failed: {error}",
                    Cancel = "OK"
                });
                if (success)
                {
                    await Initialize(Order.OrderKey);
                }
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = $"Could not close page: {ex.Message}",
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
