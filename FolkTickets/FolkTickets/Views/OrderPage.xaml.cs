using FolkTickets.Helpers;
using FolkTickets.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FolkTickets.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OrderPage : ContentPage
	{
        private OrderViewModel ViewModel;
		public OrderPage(OrderViewModel viewModel)
		{
			InitializeComponent ();

            BindingContext = ViewModel = viewModel;
        }

        public async void CloseClicked()
        {
            bool closePage = await DisplayAlert("Close", "Do you really want to close this page?", "Yes", "No");
            if (closePage)
            {
                ViewModel.CloseClicked.Execute(null);
            }
        }

        private void TicketTapped(object sender, ItemTappedEventArgs e)
        {
            ViewModel.TicketClicked.Execute(e.Item);
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
        }
    }
}