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
    public class BalFolkOrderViewModel : BaseViewModel
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
        public ICommand CloseClicked { get; protected set; }
        public ICommand UpdateClicked { get; protected set; }
        public ICommand InfoClicked { get; protected set; }

        public BalFolkOrderViewModel(string key) : base()
        {
            Title = "Order";

            CloseClicked = new Command(ClosePage);
            UpdateClicked = new Command(UpdateTickets);
            InfoClicked = new Command(ShowInfo);

            Initialize(key);
        }

        private void ShowInfo(object obj)
        {
            if(Order == null)
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
                string.Join(Environment.NewLine, Order.OrderNotes.Select(n => $"{n.date_created.date.TrimEnd(new char[] { '0'})} ({n.added_by}):{Environment.NewLine}{n.content}")));

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

        private void UpdateTickets()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
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
