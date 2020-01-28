using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BarcodeScanSF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Search : ContentPage
    {
        public ForceClient client;
        public ObservableCollection<string> ItemNames { get; set; }

        public Search()
        {
            run();
            InitializeComponent();
        }

        public async void run()
        {
            ItemNames = new ObservableCollection<string>();
            await Login();
            var QueryProducts = await client.QueryAllAsync<Product>("SELECT Name FROM Product2");
            foreach (var product in QueryProducts.Records)
            {
                ItemNames.Add(product.Name);
            }
            AutoComplete.ItemsSource = ItemNames;
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
        public class Product
        {
            public string Name { get; set; }
        }
    }
}