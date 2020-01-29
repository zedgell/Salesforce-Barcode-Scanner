using Salesforce.Common;
using Salesforce.Force;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BarcodeScanSF
{
    internal class MainViewModel
    {
        public ForceClient client;

        public bool IsLoggedin = false;

        private Product _oldProduct;

        public MainViewModel()
        {
            Run();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> ItemNames { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public bool ShowLoginText { get; set; } = false;
        public bool ShowSearchButton { get; set; } = false;

        public async void GetProducts()
        {
            Products = new ObservableCollection<Product>();
            ItemNames = new ObservableCollection<string>();
            await Login();
            var QueryProducts = await client.QueryAllAsync<Product>("SELECT Id,Name,Description FROM Product2");
            foreach (var product in QueryProducts.Records)
            {
                Product P = new Product
                {
                    Name = product.Name,
                    Id = product.Id,
                    Description = product.Description
                };
                Products.Add(P);
                ItemNames.Add(product.Name);
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

        public void Run()
        {
            CheckIfLoggedIn();
            if (IsLoggedin)
            {
                ShowSearchButton = true;
                ShowLoginText = false;
                GetProducts();
            }
            else
            {
                ShowSearchButton = false;
                ShowLoginText = true;
            }
        }

        public void ShowOrHidePoducts(Product product)
        {
            if (_oldProduct == product)
            {
                product.IsVisible = !product.IsVisible;
                UpdateProducts(product);
            }
            else
            {
                if (_oldProduct != null)
                {
                    _oldProduct.IsVisible = false;
                    UpdateProducts(_oldProduct);
                }
                product.IsVisible = true;
                UpdateProducts(product);
            }

            _oldProduct = product;
        }

        private void CheckIfLoggedIn()
        {
            if (Application.Current.Properties.ContainsKey("IsLogIn"))
            {
                if (Application.Current.Properties["IsLogIn"].ToString() == "False")
                {
                    IsLoggedin = false;
                }
                else
                {
                    IsLoggedin = true;
                }
            }
            else
            {
                IsLoggedin = false;
            }
        }

        private void UpdateProducts(Product product)
        {
            var index = Products.IndexOf(product);
            Products.Remove(product);
            Products.Insert(index, product);
        }
    }
}