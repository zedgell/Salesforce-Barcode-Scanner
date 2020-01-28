using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace BarcodeScanSF
{
    public class SearchModel : INotifyPropertyChanged
    {
        private string _UserName;
        private string _Token;
        private string _Password;
        private string _SecretKey;
        public string _ConsumerKey;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        public string Token { get => _Token; set { _Token = value; OnPropertyChanged(); } }
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        public string SecretKey { get => _SecretKey; set { _SecretKey = value; OnPropertyChanged(); } }
        public string ConsumerKey { get => _ConsumerKey; set { _ConsumerKey = value; OnPropertyChanged(); } }
        public bool IsValidated { get; set; }
        public Command SubmitCommand { get; set; }

        public void SearchViewModel()
        {
            SubmitCommand = new Command(Submit);
        }
        void Submit()
        {
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
