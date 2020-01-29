using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanSF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPage : ContentPage
    {
        public Product product;
        public ForceClient client;

        public EditPage()
        {
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey("EditBarcode"))
            {
                NameText.Text = Application.Current.Properties["EditName"].ToString();
                DescText.Text = Application.Current.Properties["EditDesc"].ToString();
                IdText.Text = Application.Current.Properties["EditId"].ToString();
                ProCode.Text = Application.Current.Properties["EditBarcode"].ToString();
                Application.Current.Properties.Remove("EditBarcode");
            }
        }


        public async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            Product p = new Product
            {
                Name = NameText.Text,
                Description = DescText.Text,
                Id = IdText.Text,
                ProductCode = ProCode.Text
            };
            string id = p.Id;
            p.Id = null;
            await Login();
            var success = await client.UpdateAsync("Product2", id, p);
            if(success != null)
            {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PushAsync(new MainPage());
                    DisplayAlert("Success", "We Have Update The Product", "OK");
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => {
                    DisplayAlert("Error", "Houstan we have a error", "OK");
                });
            }
        }
        public async Task<string> Login()
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
            client = new ForceClient(instanceUrl, accessToken, apiVersion);
            return "Done";
        }

        public async void ScanBarcode_Clicked(object sender, EventArgs e)
        {
            if (Application.Current.Properties.ContainsKey("EditBarcode"))
            {
                Application.Current.Properties.Remove("EditBarcode");
            }
            await Application.Current.SavePropertiesAsync();
            var scanPage = new ZXingScannerPage();
            scanPage.OnScanResult += (result) => {
                // Stop scanning
                scanPage.IsScanning = false;
                string BarcodeVar = result.Text;
                Application.Current.Properties["EditName"] = NameText.Text;
                Application.Current.Properties["EditDesc"] = DescText.Text;
                Application.Current.Properties["EditId"] = IdText.Text;
                Application.Current.Properties["EditBarcode"] = BarcodeVar;
                Application.Current.SavePropertiesAsync();
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PushAsync(new EditPage());
                    DisplayAlert("Scanned Barcode", BarcodeVar, "OK");
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        public class Product
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductCode { get; set; }
            public string Id { get; set; }
        }
    }
}