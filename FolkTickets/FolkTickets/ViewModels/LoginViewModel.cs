using FolkTickets.Helpers;
using FolkTickets.Views;
using System;
using System.Collections.Generic;
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
        private string _PageUriInput { get; set; }
        public ICommand LoginClicked { get; protected set; }
        public string PageUri
        {
            get
            {
                return _PageUriInput;
            }
            set
            {
                if (EqualityComparer<string>.Default.Equals(_PageUriInput, value))
                    return;
                _PageUriInput = value;
                OnPropertyChanged();
            }
        }
        private string _ApiKey { get; set; }
        public string ApiKey
        {
            get
            {
                return _ApiKey;
            }
            set
            {
                if (EqualityComparer<string>.Default.Equals(_ApiKey, value))
                    return;
                _ApiKey = value;
                OnPropertyChanged();
            }
        }
        private string _ApiSecret { get; set; }
        public string ApiSecret
        {
            get
            {
                return _ApiSecret;
            }
            set
            {
                if (EqualityComparer<string>.Default.Equals(_ApiSecret, value))
                    return;
                _ApiSecret = value;
                OnPropertyChanged();
            }
        }
        public LoginViewModel()
        {
            Title = "Login";

            LoginClicked = new Command<bool>(Login);
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
                if (!useUserInput)
                {
                    userAccount = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
                }
                else
                {
                    userAccount = new Account()
                    {
                        Username = PageUri
                    };

                    userAccount.Properties.Add("ApiKey", ApiKey);
                    userAccount.Properties.Add("ApiSecret", ApiSecret);
                };

                if (userAccount == null)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(userAccount.Username))
                {
                    throw new ArgumentException("Page URI cannot be empty");
                }
                if (!userAccount.Properties.ContainsKey("ApiKey") || string.IsNullOrWhiteSpace(userAccount.Properties["ApiKey"]))
                {
                    throw new ArgumentException("API Key cannot be empty");
                }
                if (!userAccount.Properties.ContainsKey("ApiSecret") || string.IsNullOrWhiteSpace(userAccount.Properties["ApiSecret"]))
                {
                    throw new ArgumentException("API Secret cannot be empty");
                }

                // Test connection
                await TestWCConnectionAsync(userAccount.Username, userAccount.Properties["ApiKey"], userAccount.Properties["ApiSecret"]);

                // Save credentials if the connection succeeded
                AccountStore.Create(Forms.Context).Save(userAccount, App.AppName);

                MessagingCenter.Send(this, "Error", new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = "Logged succesfully",
                    Cancel = "OK"
                });
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

        private async Task TestWCConnectionAsync(string uri, string apiKey, string apiSecret)
        {
            RestAPI rest = new RestAPI(uri,
                    apiKey,
                    apiSecret,
                    true,
                    null,
                    null,
                    null,
                    null,
                    new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Accept-Language", "pl_PL") });
            if(rest == null)
            {
                throw new Exception("Could initialize RestAPI object");
            }

            WCObject wcObj = new WCObject(rest);
            if(wcObj == null)
            {
                throw new Exception("Could not initialize WC object");
            }

            await wcObj.SystemStatus.GetAll();
            return;
        }
    }
}
