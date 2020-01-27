﻿using Salesforce.Common;
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
        public string consumerkey;
        public string secretKey;
        public string userName;
        public string password;
        public string instanceUrl;
        public string accessToken;
        public string apiVersion;
        public string Barcode;

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

            var UpdatePage = new UpdateItem();

            await Navigation.PushAsync(UpdatePage);
        }

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            CheckIfLoggedIn();
            var addItemPage = new AddItem();
            await Navigation.PushAsync(addItemPage);
        }

        private async void CheckIfLoggedIn()
        {
            if (Application.Current.Properties.ContainsKey("IsLogIn"))
            {
                if (Application.Current.Properties["IsLogIn"].ToString() == "false")
                {
                    Device.BeginInvokeOnMainThread(() => {
                        DisplayAlert("Error", "Please Login To Salesforce", "OK");
                    });
                    var Login = new SalesforceLoginDetails();
                    await Navigation.PushAsync(Login);
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => {
                    DisplayAlert("Error", "Please Login To Salesforce", "OK");
                });
                var Login = new SalesforceLoginDetails();
                await Navigation.PushAsync(Login);
            }
        }
    }
}
