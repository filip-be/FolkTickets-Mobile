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
                    }
                }
            };
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
            LoginPage loginPage = ((Current.MainPage as TabbedPage)?
                .Children
                .Where(c => c.Title == "Login")
                .FirstOrDefault() as NavigationPage)?
                .CurrentPage as LoginPage;

            // Try to login
            if(loginPage != null
                && (loginPage.BindingContext as LoginViewModel)?.Login(null, false).Result == true)
            {
                loginPage.PageUriInput = "Succeeded";
            }
            else
            {
                var userAccount = Xamarin.Auth.AccountStore.Create(Forms.Context).FindAccountsForService(AppName).FirstOrDefault();
                if(userAccount != null)
                {
                    loginPage.PageUriInput = userAccount.Username;
                    if(userAccount.Properties.ContainsKey("ApiKey"))
                    {
                        loginPage.ApiKeyInput = userAccount.Properties["ApiKey"];
                    }
                    if (userAccount.Properties.ContainsKey("ApiSecret"))
                    {
                        loginPage.ApiKeyInput = userAccount.Properties["ApiSecret"];
                    }
                }
            }
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
