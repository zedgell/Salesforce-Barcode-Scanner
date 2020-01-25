using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanSF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddItem : ContentPage
    {
        public string BarcodeVar;
        public string NameVar;
        public string DescVar;

        public AddItem()
        {
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey("BarcodeVar") && Application.Current.Properties.ContainsKey("NameVar") && Application.Current.Properties.ContainsKey("DescVar"))
            {
                BarcodeText.Text = Application.Current.Properties["BarcodeVar"].ToString();
                Name.Text = Application.Current.Properties["NameVar"].ToString();
                Desc.Text = Application.Current.Properties["DescVar"].ToString();
                BarcodeText.IsVisible = true;
                EnterManual.IsVisible = false;
                Submit.IsVisible = true;
            }
        }

        private void EnterManual_click(object sender, EventArgs e)
        {
            BarcodeText.IsVisible = true;
            EnterManual.IsVisible = false;
            Submit.IsVisible = true;
        }
        
        private async void Submit_Clicked(object sender, EventArgs e)
        {
            string consumerkey = Application.Current.Properties["ConKeyText"].ToString();
            string secretKey = Application.Current.Properties["SecretKeyText"].ToString();
            string userName = Application.Current.Properties["UserNameText"].ToString();
            string password = Application.Current.Properties["PasswordAndTokenText"].ToString();
            var auth = new AuthenticationClient();
            await auth.UsernamePasswordAsync(consumerkey, secretKey, userName, password);
            var instanceUrl = auth.InstanceUrl;
            var accessToken = auth.AccessToken;
            var apiVersion = auth.ApiVersion;
            var client = new ForceClient(instanceUrl, accessToken, apiVersion);
            var product = new { Name = Name.Text, Description = Desc.Text, ProductCode = BarcodeText.Text };
            var Products = await client.QueryAsync<Product>("SELECT Name FROM Product2 WHERE ProductCode = '" + BarcodeText.Text + "'");
            if(Products.Records.Count == 0)
            {
                var id = await client.CreateAsync("Product2", product);
                Submit.IsEnabled = false;
                if (id != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Created Successfuly", "Your Product Was Succesfully Added", "ok");
                        Submit.IsEnabled = true;
                        Name.Text = null;
                        Desc.Text = null;
                        BarcodeText.Text = null;
                    });
                    if (Application.Current.Properties.ContainsKey("BarcodeVar"))
                    {
                        Application.Current.Properties.Remove("BarcodeVar");
                        Application.Current.Properties.Remove("NameVar");
                        Application.Current.Properties.Remove("DescVar");
                    }
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Error", "There Was A Problem With Creating Your Product", "ok");
                    });
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "A Product With That Barcode Already Exsist", "ok");
                });
            }
        }

        private async void Scan_CLicked(object sender, EventArgs e)
        {
            Application.Current.Properties["NameVar"] = Name.Text;
            Application.Current.Properties["DescVar"] = Desc.Text;
            await Application.Current.SavePropertiesAsync();
            var scanPage = new ZXingScannerPage();
            scanPage.OnScanResult += (result) => {
                // Stop scanning
                scanPage.IsScanning = false;
                BarcodeVar = result.Text;
                Application.Current.Properties["BarcodeVar"] = BarcodeVar;
                Application.Current.SavePropertiesAsync();
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PushAsync(new AddItem());
                    DisplayAlert("Scanned Barcode", BarcodeVar, "OK");
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        public class Product
        {
            public string Name { get; set; }
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            var LoginPage = new SalesforceLoginDetails();
            await Navigation.PushAsync(LoginPage);
        }

        private async void Home_clicked(object sender, EventArgs e)
        {
            var HomePage = new MainPage();
            await Navigation.PushAsync(HomePage);
        }

        private async void Update_clicked(object sender, EventArgs e)
        {
            //TODO
        }

    }
}