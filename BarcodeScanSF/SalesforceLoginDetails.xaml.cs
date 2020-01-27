﻿using Salesforce.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BarcodeScanSF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalesforceLoginDetails : ContentPage
    {
        public string ConKeyText;
        public string UserNameText;
        public string PasswordAndTokenText;
        public string SecretKeyText;

        public SalesforceLoginDetails()
        {
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey("Password"))
            {
                UserName.Text = Application.Current.Properties["UserNameText"].ToString();
                Password.Text = Application.Current.Properties["Password"].ToString();
                Token.Text = Application.Current.Properties["TokenText"].ToString();
                SecretKey.Text = Application.Current.Properties["SecretKeyText"].ToString();
                ConsumerKey.Text = Application.Current.Properties["ConKeyText"].ToString();

                if(Application.Current.Properties["IsLogIn"].ToString().ToLower() == "true")
                {
                    Edit(false);
                }

            }
            else
            {
                Application.Current.Properties["UserNameText"] = "";
                Application.Current.Properties["Password"] = "";
                Application.Current.Properties["TokenText"] = "";
                Application.Current.Properties["SecretKeyText"] = "";
                Application.Current.Properties["ConKeyText"] = "";
                Application.Current.Properties["IsLogIn"] = false;
                Application.Current.SavePropertiesAsync();
            }
        }

        private async void Login(object sender, EventArgs e)
        {
            ConKeyText = ConsumerKey.Text;
            UserNameText = UserName.Text;
            PasswordAndTokenText = Password.Text + Token.Text;
            SecretKeyText = SecretKey.Text;
            var auth = new AuthenticationClient();
            int timeout = 2000;
            var task = auth.UsernamePasswordAsync(ConKeyText, SecretKeyText, UserNameText, PasswordAndTokenText);
            if(await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                if(auth.AccessToken != null)
                {
                    Application.Current.Properties["ConKeyText"] = ConKeyText;
                    Application.Current.Properties["UserNameText"] = UserNameText;
                    Application.Current.Properties["Password"] = Password.Text;
                    Application.Current.Properties["TokenText"] = Token.Text;
                    Application.Current.Properties["PasswordAndTokenText"] = PasswordAndTokenText;
                    Application.Current.Properties["SecretKeyText"] = SecretKeyText;
                    Application.Current.Properties["IsLogIn"] = true;
                    await Application.Current.SavePropertiesAsync();
                    var Main = new MainPage
                    {
                        consumerkey = ConKeyText,
                        secretKey = SecretKeyText,
                        userName = UserNameText,
                        password = PasswordAndTokenText
                    };
                    Device.BeginInvokeOnMainThread(() => {
                        DisplayAlert("LoggedIn", "LoggedIn Successfully", "OK");
                    });
                    await Navigation.PushAsync(Main);
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => {
                        DisplayAlert("Error", "Loggin was Unsuccessfull", "OK");
                    });
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => {
                    DisplayAlert("Error", "Loggin was Unsuccessfull", "OK");
                });
            }
        }

        private void Edit_clicked(object sender, EventArgs e)
        {
            Edit(true);
        }
        
        private void Edit(bool IsEdit)
        {
            if (IsEdit)
            {
                UserName.IsReadOnly = false;
                Token.IsReadOnly = false;
                Password.IsReadOnly = false;
                ConsumerKey.IsReadOnly = false;
                SecretKey.IsReadOnly = false;
                Edit_Button.IsVisible = false;
                login.IsVisible = true;
            }
            else
            {
                UserName.IsReadOnly = true;
                Token.IsReadOnly = true;
                Password.IsReadOnly = true;
                ConsumerKey.IsReadOnly = true;
                SecretKey.IsReadOnly = true;
                Edit_Button.IsVisible = true;
                login.IsVisible = false;
            }
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

        private async void Home_clicked(object sender, EventArgs e)
        {
            var HomePage = new MainPage();
            await Navigation.PushAsync(HomePage);
        }

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            CheckIfLoggedIn();
            var addItemPage = new AddItem();
            await Navigation.PushAsync(addItemPage);
        }

        private async void Update_clicked(object sender, EventArgs e)
        {
            CheckIfLoggedIn();

            var UpdatePage = new UpdateItem();

            await Navigation.PushAsync(UpdatePage);
        }


    }
}