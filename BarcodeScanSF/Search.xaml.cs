using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BarcodeScanSF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Search : ContentPage
    {
        public ForceClient client;
        public string id;
        public Product product;
        public string SearchedName;
        private Product _oldProduct;

        public Search()
        {
            Run();
            InitializeComponent();
        }

        public ObservableCollection<string> ItemNames { get; set; }
        public ObservableCollection<Product> Products { get; set; }

        public async void EditButton_Clicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            id = btn.ClassId;
            var itemfound = await QueryId();
            if (itemfound)
            {
                var editPage = new EditPage
                {
                    BindingContext = product
                };
                await Navigation.PushAsync(editPage);
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
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

            if (product != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async void Run()
        {
            ItemNames = new ObservableCollection<string>();
            Products = new ObservableCollection<Product>();
            await Login();
            var QueryProducts = await client.QueryAllAsync<Product>("SELECT Name,Description FROM Product2");
            foreach (var product in QueryProducts.Records)
            {
                Product p = new Product
                {
                    Name = product.Name,
                    Description = product.Description
                };
                Products.Add(p);
                ItemNames.Add(p.Name);
            }
            AutoComplete.ItemsSource = ItemNames;
        }

        public async void RunCustonQuery()
        {
            Products.Clear();
            await Login();
            var QueryProducts = await client.QueryAllAsync<Product>("SELECT Name,Description FROM Product2 WHERE Name LIKE '%" + SearchedName + "%'");
            foreach (var product in QueryProducts.Records)
            {
                Product Pro = new Product
                {
                    Name = product.Name,
                    Description = product.Description
                };
                Products.Add(Pro);
            }
            ProductSeen.ItemsSource = Products;
            ProductSeen.IsVisible = true;
            SearchStack.IsVisible = false;
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

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var product = e.Item as Product;
            ShowOrHidePoducts(product);
        }

        private void Search_clicked(object sender, EventArgs e)
        {
            SearchedName = AutoComplete.Text;
            if (ItemNames.Contains(SearchedName))
            {
                foreach (var product in Products)
                {
                    if (product.Name == SearchedName)
                    {
                        Products.Clear();
                        Product pro = new Product
                        {
                            Name = product.Name,
                            Description = product.Description
                        };
                        Products.Add(pro);
                        SearchAgain.IsVisible = true;
                        ProductSeen.ItemsSource = Products;
                        ProductSeen.IsVisible = true;
                        SearchStack.IsVisible = false;
                        break;
                    }
                }
            }
            else
            {
                RunCustonQuery();
                SearchAgain.IsVisible = true;
            }
        }

        private void SearchAgain_CLicked(object sender, EventArgs e)
        {
            ProductSeen.IsVisible = false;
            SearchStack.IsVisible = true;
            SearchAgain.IsVisible = false;
        }

        private void UpdateProducts(Product product)
        {
            var index = Products.IndexOf(product);
            Products.Remove(product);
            Products.Insert(index, product);
        }
    }
}