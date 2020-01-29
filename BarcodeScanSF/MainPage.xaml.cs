using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanSF
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        public bool IsLoggedin = false;
        public string consumerkey;
        public string secretKey;
        public string userName;
        public string password;
        public string instanceUrl;
        public string accessToken;
        public string apiVersion;
        public string Barcode;
        public string id;
        public Product product;
        public ForceClient client;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            var LoginPage = new SalesforceLoginDetails();

            await Navigation.PushAsync(LoginPage);
        }

        private async void Update_clicked(object sender, EventArgs e)
        {
            CheckIfLoggedIn();

            if (IsLoggedin)
            {
                var UpdatePage = new UpdateItem();

                await Navigation.PushAsync(UpdatePage);
            }
        }

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            CheckIfLoggedIn();
            if (IsLoggedin)
            {
                var addItemPage = new AddItem();
                await Navigation.PushAsync(addItemPage);
            }
          
        }

        private async void Search_Clicked(object sender, EventArgs e)
        {
            CheckIfLoggedIn();
            if (IsLoggedin)
            {
                var SearchPage = new Search();
                await Navigation.PushAsync(SearchPage);
            }

        }

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var product = e.Item as Product;
            var vm = BindingContext as MainViewModel;
            vm?.ShowOrHidePoducts(product);
        }

        private async void CheckIfLoggedIn()
        {
            if (Application.Current.Properties.ContainsKey("IsLogIn"))
            {
                if (Application.Current.Properties["IsLogIn"].ToString() == "False")
                {
                    Device.BeginInvokeOnMainThread(() => {
                        DisplayAlert("Error", "Please Login To Salesforce", "OK");
                    });
                    var Login = new SalesforceLoginDetails();
                    await Navigation.PushAsync(Login);
                    IsLoggedin = false;
                }
                else
                {
                    IsLoggedin = true;
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => {
                    DisplayAlert("Error", "Please Login To Salesforce", "OK");
                });
                var Login = new SalesforceLoginDetails();
                await Navigation.PushAsync(Login);
                IsLoggedin = false;
            }
        }
        public async void EditButton_Clicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            id = btn.ClassId;
            var itemfound = await QueryId();
            if (itemfound)
            {
                Console.WriteLine("Desc is: " + product.Description);
                var editPage = new EditPage
                {
                    BindingContext = product
                };
                await Navigation.PushAsync(editPage);
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => {
                    DisplayAlert("Error", "Houstan we have a error", "OK");
                });
            }
        }

        public async Task<bool> QueryId()
        {
            await Login();
            var QueryProducts = await client.QueryAllAsync<Product>("SELECT Id,Name,Description,ProductCode FROM Product2 Where Name = '" + id + "'");
            Product p = new Product
            {
                Name = QueryProducts.Records[0].Name,
                Description = QueryProducts.Records[0].Description,
                Id = QueryProducts.Records[0].Id,
                ProductCode = QueryProducts.Records[0].ProductCode
            };

            product = p;

            if(product != null)
            {
                return true;
            }
            else
            {
                return false;
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
    }
}
