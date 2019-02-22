using FolkTickets.ViewModels;
using FolkTickets.Views;
using Plugin.Iconize;
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

        public App(string inputFile)
        {
            InitializeComponent();

            var tabbedPage = new IconTabbedPage
            {
                Children =
                {
                    new LoginPage(inputFile, true)
                    {
                        Title = "Login",
                        Icon = "fas-sign-in-alt"
                    },
                }
            };

            MainPage = new IconNavigationPage(tabbedPage);
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
