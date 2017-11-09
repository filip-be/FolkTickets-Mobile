using FolkTickets.ViewModels;
using FolkTickets.Views;
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

		public App ()
		{
			InitializeComponent();

            Current.MainPage = new TabbedPage
            {
                Children =
                {
                    new NavigationPage(new LoginPage())
                    {
                        Title = "Login"
                    },
                    new NavigationPage(new OrdersPage())
                    {
                        Title = "Orders"
                    }
                }
            };
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
