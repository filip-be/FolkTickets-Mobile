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
	public partial class BalFolkOrderPage : ContentPage
	{
        private BalFolkOrderViewModel ViewModel;
		public BalFolkOrderPage (BalFolkOrderViewModel viewModel)
		{
			InitializeComponent ();

            BindingContext = ViewModel = viewModel;

            MessagingCenter.Subscribe<BalFolkOrderViewModel, MessagingCenterAlert>(this, "Error", async (sender, item) =>
            {
                await DisplayAlert(item.Title, item.Message, item.Cancel);
            });
        }

        private void TicketTapped(object sender, ItemTappedEventArgs e)
        {

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<LoginViewModel, MessagingCenterAlert>(this, "Error");
        }
    }
}