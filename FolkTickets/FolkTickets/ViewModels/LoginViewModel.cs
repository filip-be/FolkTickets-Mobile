using FolkTickets.Helpers;
using FolkTickets.Services;
using FolkTickets.Views;
using FormsPlugin.Iconize;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v2;
using Xamarin.Auth;
using Xamarin.Forms;

namespace FolkTickets.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _PageUriInput = string.Empty;
        public ICommand LoginClicked { get; protected set; }
        public string PageUri
        {
            get
            {
                return _PageUriInput;
            }
            set
            {
                SetProperty(ref _PageUriInput, value);
            }
        }
        private string _ApiKey = string.Empty;
        public string ApiKey
        {
            get
            {
                return _ApiKey;
            }
            set
            {
                SetProperty(ref _ApiKey, value);
            }
        }
        private string _ApiSecret = string.Empty;
        public string ApiSecret
        {
            get
            {
                return _ApiSecret;
            }
            set
            {
                SetProperty(ref _ApiSecret, value);
            }
        }
        private CultureInfo _Language;
        public CultureInfo Language
        {
            get
            {
                return _Language;
            }
            set
            {
                SetProperty(ref _Language, value);
            }
        }
        public ICollection<CultureInfo> Languages { get; private set; }
        public LoginViewModel() : base()
        {
            Title = "Login";

            LoginClicked = new Command<bool>(Login);

            Languages = CultureInfo
                    .GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)
                    .OrderBy(c => c.DisplayName)
                    .ToArray();
            string currentLang = null;

            Account userAccount = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
            if(userAccount != null)
            {
                PageUri = userAccount.Username;
                if (userAccount.Properties.ContainsKey("ApiKey"))
                {
                    ApiKey = userAccount.Properties["ApiKey"];
                }
                if (userAccount.Properties.ContainsKey("ApiSecret"))
                {
                    ApiSecret = userAccount.Properties["ApiSecret"];
                }
                if (userAccount.Properties.ContainsKey("Lang"))
                {
                    currentLang = userAccount.Properties["Lang"];
                }
            }

            if (!string.IsNullOrEmpty(currentLang))
            {
                Language = Languages.Where(c => c.Name == currentLang).FirstOrDefault();
            }
            if (Language == null)
            {
                Language = CultureInfo.CurrentCulture;
            }
        }

        /// <summary>
        /// Login to the WC
        /// </summary>
        /// <param name="page">View</param>
        /// <param name="useUserInput">Use user input</param>
        /// <param name="displayErrors">Display errors to the view</param>
        private async void Login(bool useUserInput)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Account userAccount = null;

            try
            {
                // Get user account from app store
                userAccount = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
                if (useUserInput)
                {
                    if (userAccount == null)
                    {
                        userAccount = new Account();
                    }

                    userAccount.Username = PageUri;

                    if (userAccount.Properties.ContainsKey("ApiKey"))
                    {
                        userAccount.Properties["ApiKey"] = ApiKey;
                    }
                    else
                    {
                        userAccount.Properties.Add("ApiKey", ApiKey);
                    }
                    if (userAccount.Properties.ContainsKey("ApiSecret"))
                    {
                        userAccount.Properties["ApiSecret"] = ApiSecret;
                    }
                    else
                    {
                        userAccount.Properties.Add("ApiSecret", ApiSecret);
                    }
                    if (userAccount.Properties.ContainsKey("Lang"))
                    {
                        userAccount.Properties["Lang"] = Language?.Name;
                    }
                    else
                    {
                        userAccount.Properties.Add("Lang", Language?.Name);
                    }
                };

                if (userAccount == null)
                {
                    return;
                }

                await WCService.TestCredentialsAsync(true, userAccount);

                // Save credentials if the connection succeeded
                AccountStore.Create(Forms.Context).Save(userAccount, App.AppName);

                if(!(App.Current.MainPage is IconNavigationPage)
                    || !((App.Current.MainPage as IconNavigationPage)?.CurrentPage is IconTabbedPage))
                {
                    MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                    {
                        Title = "Error",
                        Message = "Logged succesfully, but there are invalid application pages. Cannot proceed!",
                        Cancel = "OK"
                    });
                    return;
                }

                IconTabbedPage tabbedPage = (App.Current.MainPage as IconNavigationPage).CurrentPage as IconTabbedPage;

                tabbedPage.Children.Clear();
                tabbedPage.Children.Add(new OrdersPage());
                
                return;
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = string.Format("Unable to login: {0}", ex.Message),
                    Cancel = "OK"
                });
                return;
            }
            finally
            {
                if (userAccount != null)
                {
                    PageUri = userAccount.Username;
                    if (userAccount.Properties.ContainsKey("ApiKey"))
                    {
                        ApiKey = userAccount.Properties["ApiKey"];
                    }
                    if (userAccount.Properties.ContainsKey("ApiSecret"))
                    {
                        ApiSecret = userAccount.Properties["ApiSecret"];
                    }
                }
                IsBusy = false;
            }
        }
    }
}
