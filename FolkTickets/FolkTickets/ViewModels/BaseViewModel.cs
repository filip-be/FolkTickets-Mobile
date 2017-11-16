using FolkTickets.Helpers;
using FolkTickets.Views;
using FormsPlugin.Iconize;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FolkTickets.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// Get the azure service instance
        /// </summary>
        //public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();


        public ICommand SignOutCommand { get; set; }
        public BaseViewModel()
        {
            SignOutCommand = new Command(SignOut);
        }

        private void SignOut()
        {
            if(App.Current.MainPage is IconNavigationPage
                && (App.Current.MainPage as IconNavigationPage)?.CurrentPage is IconTabbedPage)
            {
                IconTabbedPage tabbedPage = (App.Current.MainPage as NavigationPage).CurrentPage as IconTabbedPage;
                tabbedPage.Children.Clear();
                tabbedPage.Children.Add(new LoginPage(false));
            }
        }

        /// <summary>
        /// Private field to hold isBusy value
        /// </summary>
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { IsControlsVisible = !value; SetProperty(ref isBusy, value); }
        }
        bool isControlsVisible = true;
        public bool IsControlsVisible
        {
            get { return isControlsVisible; }
            set { SetProperty(ref isControlsVisible, value); }
        }
        /// <summary>
        /// Private backing field to hold the title
        /// </summary>
        string title = string.Empty;
        /// <summary>
        /// Public property to set and get the title of the item
        /// </summary>
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
    }
}
