using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
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
	public partial class OrderNotePage : PopupPage
	{
        public bool SaveNote { get; set; } = false;
        public string Note
        {
            get
            {
                return NoteText?.Text;
            }
        }
		public OrderNotePage () : base()
		{
			InitializeComponent ();
            CloseWhenBackgroundIsClicked = true;
        }

        private void AddClicked(object sender, EventArgs e)
        {
            SaveNote = true;
            Navigation.PopPopupAsync();
        }
    }
}