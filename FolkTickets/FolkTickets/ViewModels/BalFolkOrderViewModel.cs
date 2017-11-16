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
        public MobileOrder Order { get; protected set; }
        public ICommand CloseClicked { get; protected set; }
        public ICommand UpdateClicked { get; protected set; }

        public BalFolkOrderViewModel() : base()
        {
            Title = "Order";

            CloseClicked = new Command(ClosePage);
            UpdateClicked = new Command(UpdateTickets);
        }

        public async Task Initialize(string key)
        {
            Order = await WCService.GetBFTOrder(key);
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
