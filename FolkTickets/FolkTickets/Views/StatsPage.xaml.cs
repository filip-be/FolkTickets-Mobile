using FolkTickets.Helpers;
using FolkTickets.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FolkTickets.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatsPage : ContentPage
	{
        private StatsViewModel ViewModel;

        public StatsPage ()
		{
			InitializeComponent ();

            BindingContext = ViewModel = new StatsViewModel();
            ViewModel.RefreshClicked.Execute(null);
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<StatsViewModel, MessagingCenterAlert>(this, "Error", async (sender, item) =>
            {
                await DisplayAlert(item.Title, item.Message, item.Cancel);
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<StatsViewModel, MessagingCenterAlert>(this, "Error");
        }
    }
}