using FolkTickets.ViewModels;
using FolkTickets.Views;
using FormsPlugin.Iconize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace FolkTickets
{
    public partial class App : Application
    {
        public static string AppName { get; private set; } = "FolkTickets";

        public App()
        {
            InitializeComponent();
            
            var tabbedPage = new IconTabbedPage
            {
                Children =
                {
                    new LoginPage()
                    {
                        Title = "Login",
                        Icon = "fa-sign-in"
                    },
                    new OrdersPage()
                    {
                        Title = "Orders",
                        Icon = "fa-tasks"
                    }
                }
            };

            Current.MainPage = new IconNavigationPage(tabbedPage);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
