
using FolkTickets.Helpers;
using FolkTickets.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v2;
using WooCommerceNET.WooCommerce.v2.Extension;
using Xamarin.Auth;
//using Xamarin.Auth;
using Xamarin.Forms;

namespace FolkTickets.Views
{
	public partial class LoginPage : ContentPage
	{
        private LoginViewModel ViewModel;

        public string ApiKeyInput { get; set; }
        public string ApiSecretInput { get; set; }
        public string PageUriInput { get; set; }

        public LoginPage()
		{
			InitializeComponent();

            BindingContext = ViewModel = new LoginViewModel();

            MessagingCenter.Subscribe<MessagingCenterAlert>(this, "Error", async (item) =>
            {
                await DisplayAlert(item.Title, item.Message, item.Cancel);
            });
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            try
            {
                Account acc = new Account
                {
                    Username = PageUri.Text
                };
                acc.Properties.Add("ApiKey", ApiKey.Text);
                acc.Properties.Add("ApiSecret", ApiSecret.Text);

                MessagingCenter.Send(this, "Login", acc);
                await Navigation.PopToRootAsync();
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", string.Format("Unable to login: {0}", ex.Message), "OK");
            }
        }
    }
}
