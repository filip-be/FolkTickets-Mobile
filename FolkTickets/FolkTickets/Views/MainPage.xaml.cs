using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v2;
using WooCommerceNET.WooCommerce.v2.Extension;
using Xamarin.Forms;

namespace FolkTickets.Views
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        private void ButtonLogin_Clicked(object sender, EventArgs e)
        {
            try
            {
                string sProducts = string.Empty;
                WelcomeLabel.Text = sProducts;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Empty;
                do
                {
                    errorMessage = string.Format("{0}{1}{2}", errorMessage, ex.Message, Environment.NewLine);
                } while ((ex = ex.InnerException) != null);
                WelcomeLabel.Text = errorMessage;
            }
        }
    }
}
