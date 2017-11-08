using FolkTickets.Helpers;
using FolkTickets.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v2;
using Xamarin.Auth;
using Xamarin.Forms;

namespace FolkTickets.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel()
        {
            Title = "Login";
            
            MessagingCenter.Subscribe<LoginPage, Account>(this, "Login", Login);
        }

        public async Task<bool> Login(Account userAccount, bool displayErrors = true)
        {
            if (IsBusy)
                return false;

            IsBusy = true;

            try
            {
                // Get user account from app store
                if (userAccount == null)
                {
                    userAccount = AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
                }

                if (userAccount == null)
                {
                    return false;
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
                return true;
            }
            catch (Exception ex)
            {
                if (displayErrors)
                {
                    MessagingCenter.Send(new MessagingCenterAlert
                    {
                        Title = "Error",
                        Message = string.Format("Unable to login: {0}", ex.Message),
                        Cancel = "OK"
                    }, "Error");
                }
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void Login(LoginPage page, Account userAccount)
        {
            await Login(userAccount);
            return;
        }

        public async Task TestWCConnectionAsync(string uri, string apiKey, string apiSecret)
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
