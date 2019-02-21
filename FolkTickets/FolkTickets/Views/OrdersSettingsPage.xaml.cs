using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkTickets.Helpers;
using FolkTickets.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FolkTickets.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OrdersSettingsPage : ContentPage
	{
        private OrdersSettingsViewModel ViewModel;

		public OrdersSettingsPage (OrdersSettingsViewModel viewModel)
		{
			InitializeComponent ();

            BindingContext = ViewModel = viewModel;
            viewModel.RefreshSettings.Execute(null);
        }

        private void SettingTapped(object sender, ItemTappedEventArgs e)
        {
            ViewModel.SettingClicked.Execute(e.Item);
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<OrderViewModel, MessagingCenterAlert>(this, "Error", async (sender, item) =>
            {
                await DisplayAlert(item.Title, item.Message, item.Cancel);
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<OrderViewModel, MessagingCenterAlert>(this, "Error");
            MessagingCenter.Unsubscribe<OrderViewModel, MessagingCenterAlert>(this, "Note");
        }
    }
}