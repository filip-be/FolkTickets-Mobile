using FolkTickets.Helpers;
using FolkTickets.ViewModels;
using Xamarin.Forms;

namespace FolkTickets.Views
{
	public partial class LoginPage : ContentPage
	{
        private LoginViewModel ViewModel;
        
        public LoginPage(string inputFile, bool tryToLogin)
		{
			InitializeComponent();

            BindingContext = ViewModel = new LoginViewModel(inputFile);
            
            if (tryToLogin)
            {
                ViewModel.LoginClicked.Execute(!string.IsNullOrWhiteSpace(inputFile));
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
