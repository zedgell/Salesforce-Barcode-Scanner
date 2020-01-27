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
    public partial class UpdateItem : ContentPage
    {
        public ForceClient client;
        public string LookupVar;
        public string LookupType;
        public string id;
        public bool BarcodeScan;
        public int CurrentSelected { get; } = -1;
        Product ProductToUpdate;

        public UpdateItem()
        {
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey("BarcodeVar"))
            {
                CurrentSelected = 1;
                BindingContext = this;
                BarcodeText.Text = Application.Current.Properties["BarcodeVar"].ToString();
            }
        }
        private async void Scan_CLicked(object sender, EventArgs e)
        {
            if (Application.Current.Properties.ContainsKey("BarcodeVar"))
            {
                Application.Current.Properties.Remove("BarcodeVar");
            }
            await Application.Current.SavePropertiesAsync();
            var scanPage = new ZXingScannerPage();
            scanPage.OnScanResult += (result) => {
                // Stop scanning
                scanPage.IsScanning = false;
                string BarcodeVar = result.Text;
                Application.Current.Properties["BarcodeVar"] = BarcodeVar;
                Application.Current.SavePropertiesAsync();
                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PushAsync(new UpdateItem());
                    DisplayAlert("Scanned Barcode", BarcodeVar, "OK");
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        private async void LookUp_Clicked(object sender, EventArgs e)
        {
            if (LookUpBy.Items[LookUpBy.SelectedIndex] == "Name")
            {
                LookupVar = Name.Text;
                LookupType = "Name";
            }
            else
            {
                LookupVar = BarcodeText.Text;
                LookupType = "ProductCode";
            }
            await Login();
            var Products = await client.QueryAsync<Product>("SELECT Id,Name,Description,ProductCode FROM Product2 WHERE " + LookupType + " = '" + LookupVar + "' LIMIT 3");
            if (Products.Records.Count != 0)
            {
                if (Application.Current.Properties.ContainsKey("BarcodeVar"))
                {
                    Application.Current.Properties.Remove("BarcodeVar");
                }
                await Application.Current.SavePropertiesAsync();
                Name.Text = Products.Records[0].Name;
                Desc.Text = Products.Records[0].Description;
                BarcodeText.Text = Products.Records[0].ProductCode;
                ProductToUpdate = Products.Records[0];
                Name.IsVisible = true;
                Desc.IsVisible = true;
                BarcodeText.IsVisible = true;
                BarcodeText.IsReadOnly = true;
                Update.IsVisible = true;
                Lookup.IsVisible = false;
                Scan.IsVisible = false;
                Grid.SetRow(Desc, 3);
                Grid.SetRow(BarcodeText, 4);
                Grid.SetRow(Update, 5);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Success", "We Found The Product", "OK");
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "We Could Find A Product That Matches", "OK");
                });
            }
        }

        private async void Update_Clicked(object sender, EventArgs e)
        {
            await Login();
            string id = ProductToUpdate.Id;
            ProductToUpdate.Name = Name.Text;
            ProductToUpdate.Description = Desc.Text;
            ProductToUpdate.ProductCode = BarcodeText.Text;
            ProductToUpdate.Id = null;
            var success = await client.UpdateAsync("Product2", id, ProductToUpdate);
            if(success != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Success", "We Updates The Product", "OK");
                    Navigation.PushAsync(new MainPage());
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Success", "We Updates The Product", "OK");
                });
            }


        }

        private void Lookup_Type_Chnaged(Object sender, EventArgs e)
        {
            if(LookUpBy.Items[LookUpBy.SelectedIndex] == "Name")
            {
                Name.IsVisible = true;
                BarcodeText.IsVisible = false;
                Scan.IsVisible = false;
            }
            else
            {
                Console.WriteLine("Lookup Number Is: " + LookUpBy.SelectedIndex);
                BarcodeText.IsVisible = true;
                Scan.IsVisible = true;
                Name.IsVisible = false;
            }
        }

        public class Product
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductCode { get; set; }
        }

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            var addItemPage = new AddItem();
            await Navigation.PushAsync(addItemPage);
        }

        private async void Home_clicked(object sender, EventArgs e)
        {
            var HomePage = new MainPage();
            await Navigation.PushAsync(HomePage);
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            var LoginPage = new SalesforceLoginDetails();

            await Navigation.PushAsync(LoginPage);
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

    }
}