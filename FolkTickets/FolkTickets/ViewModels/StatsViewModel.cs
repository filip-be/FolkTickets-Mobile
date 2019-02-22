using FolkTickets.Models;
using FolkTickets.Helpers;
using FolkTickets.Services;
using Plugin.Iconize;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FolkTickets.ViewModels
{
    public class StatsViewModel : BaseViewModel
    {
        /// <summary>
        /// Refresh command
        /// </summary>
        public ICommand RefreshClicked { get; protected set; }
        /// <summary>
        /// Close command
        /// </summary>
        public ICommand CloseClicked { get; protected set; }
        /// <summary>
        /// List of items
        /// </summary>
        public ObservableCollection<Statistic> Stats { get; protected set; }
        /// <summary>
        /// Stats list
        /// </summary>
        private IEnumerable<Statistic> StatsList { get; set; }

        public StatsViewModel() : base()
        {
            Title = "Stats";

            Stats = new ObservableCollection<Statistic>();

            RefreshClicked = new Command(RefreshStats);
            CloseClicked = new Command(ClosePage);
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

        private async void RefreshStats()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                var statistics = await WCService.GetStatistics();
                StatsList = statistics.OrderBy(s => s.Event).ThenBy(s => s.Product).ToList();
                Stats.Clear();
                foreach (var stat in StatsList.OrderBy(s => s.Product))
                {
                    Stats.Add(stat);
                }
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Could not laod statistics: {0}", ex.Message),
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
