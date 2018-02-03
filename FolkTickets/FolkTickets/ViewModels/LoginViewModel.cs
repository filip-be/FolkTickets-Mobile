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
    /// <summary>
    /// Login View Modle
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        /// <summary>
        /// Login clicked command
        /// </summary>
        public ICommand LoginClicked { get; protected set; }
        /// <summary>
        /// Private: WP URI
        /// </summary>
        private string _PageUriInput = string.Empty;
        /// <summary>
        /// WP URI
        /// </summary>
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
        /// <summary>
        /// Private: API Key
        /// </summary>
        private string _ApiKey = string.Empty;
        /// <summary>
        /// API Key
        /// </summary>
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
        /// <summary>
        /// Private: API Secret
        /// </summary>
        private string _ApiSecret = string.Empty;
        /// <summary>
        /// API Secret
        /// </summary>
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
        /// <summary>
        /// Private: Defautl language
        /// </summary>
        private CultureInfo _Language;
        /// <summary>
        /// Default language
        /// </summary>
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
        /// <summary>
        /// Private: Use SSL
        /// </summary>
        private bool _UseSSL = false;
        /// <summary>
        /// Use SSL
        /// </summary>
        public bool UseSSL
        {
            get
            {
                return _UseSSL;
            }
            set
            {
                SetProperty(ref _UseSSL, value);
            }
        }
        /// <summary>
        /// Languages list
        /// </summary>
        public ICollection<CultureInfo> Languages { get; private set; }
        /// <summary>
        /// Default constructor
        /// </summary>
        public LoginViewModel() : base()
        {
            // Page title
            Title = "Login";

            // Login clicked command
            LoginClicked = new Command<bool>(Login);

            // Load languages
            IsBusy = true;
            Languages = CultureInfo
                    .GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)
                    .OrderBy(c => c.DisplayName)
                    .ToArray();
            string currentLang = null;

            // Assign user account variables
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
                if (userAccount.Properties.ContainsKey("UseSSL"))
                {
                    bool.TryParse(userAccount.Properties["UseSSL"], out _UseSSL);
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
            IsBusy = false;
        }

        /// <summary>
        /// Login to the WC
        /// </summary>
        /// <param name="page">View</param>
        /// <param name="useUserInput">Use user input</param>
        /// <param name="displayErrors">Display errors to the view</param>
        private async void Login(bool useUserInput)
        {
            // Do not execute twice
            if (IsBusy)
                return;
            
            IsBusy = true;
            Account userAccount = null;

            try
            {
                // Get user account from app store
                userAccount = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
                // Get user input only if requested
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
                    if (userAccount.Properties.ContainsKey("UseSSL"))
                    {
                        userAccount.Properties["UseSSL"] = UseSSL.ToString();
                    }
                    else
                    {
                        userAccount.Properties.Add("UseSSL", UseSSL.ToString());
                    }
                };

                // No account found - return
                if (userAccount == null)
                {
                    return;
                }

                // Test connection to WooCommerce
                bool connectionSucceeded = await WCService.TestCredentialsAsync(true, userAccount);

                if (connectionSucceeded)
                {
                    // Remove all previously existing accounts
                    AccountStore accountStore = AccountStore.Create(Forms.Context);
                    foreach (var account in accountStore.FindAccountsForService(App.AppName))
                    {
                        accountStore.Delete(account, App.AppName);
                    }

                    // Save credentials if the connection succeeded
                    AccountStore.Create(Forms.Context).Save(userAccount, App.AppName);

                    // Check displayed page
                    if (!(App.Current.MainPage is IconNavigationPage)
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

                    // Update App pages
                    IconTabbedPage tabbedPage = (App.Current.MainPage as IconNavigationPage).CurrentPage as IconTabbedPage;

                    tabbedPage.Children.Clear();
                    tabbedPage.Children.Add(new OrdersPage());
                }
                else
                {
                    // Connection error
                    MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                    {
                        Title = "Error",
                        Message = string.Format("Unable to login: Unknown error"),
                        Cancel = "OK"
                    });
                }
                return;
            }
            catch (Exception ex)
            {
                // Connection error
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
                // Update form data
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
