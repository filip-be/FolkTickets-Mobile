using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using FormsPlugin.Iconize.Droid;
using Plugin.Iconize;

namespace FolkTickets.Droid
{
	[Activity (Label = "FolkTickets", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar; 

			base.OnCreate (bundle);

            Xamarin.Forms.Forms.Init (this, bundle);

            Iconize.With(new Plugin.Iconize.Fonts.FontAwesomeModule());
            IconControls.Init(Resource.Id.toolbar, Resource.Id.sliding_tabs);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            LoadApplication (new App());
		}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

