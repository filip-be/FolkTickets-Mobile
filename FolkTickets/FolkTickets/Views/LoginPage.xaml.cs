
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
        
        public LoginPage(bool tryToLogin = true)
		{
			InitializeComponent();

            BindingContext = ViewModel = new LoginViewModel();
            
            if (tryToLogin)
            {
                ViewModel.LoginClicked.Execute(false);
            }
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<LoginViewModel, MessagingCenterAlert>(this, "Error", async (sender, item) =>
            {
                await DisplayAlert(item.Title, item.Message, item.Cancel);
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<LoginViewModel, MessagingCenterAlert>(this, "Error");
        }

    }
}
