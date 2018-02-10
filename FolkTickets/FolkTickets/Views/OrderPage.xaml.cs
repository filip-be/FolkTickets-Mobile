using FolkTickets.Helpers;
using FolkTickets.ViewModels;
using Rg.Plugins.Popup.Extensions;
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
        private OrderNotePage NotePage { get; set; }
        public bool PopupVisible { get; set; } = false;
		public OrderPage(OrderViewModel viewModel)
		{
			InitializeComponent();

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

            MessagingCenter.Subscribe<OrderViewModel, MessagingCenterAlert>(this, "Note", async (sender, item) =>
            {
                PopupVisible = true;
                NotePage = new OrderNotePage();
                NotePage.Disappearing += NotePage_Disappearing;
                await Navigation.PushPopupAsync(NotePage);
            });
        }

        private void NotePage_Disappearing(object sender, EventArgs e)
        {
            if (NotePage != null && NotePage.SaveNote)
            {
                ViewModel.AddNoteClicked.Execute(NotePage.Note);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<OrderViewModel, MessagingCenterAlert>(this, "Error");
            MessagingCenter.Unsubscribe<OrderViewModel, MessagingCenterAlert>(this, "Note");
        }
    }
}