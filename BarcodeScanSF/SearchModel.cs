using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace BarcodeScanSF
{
    public class SearchModel : INotifyPropertyChanged
    {
        public string _ConsumerKey;
        private string _Password;
        private string _SecretKey;
        private string _Token;
        private string _UserName;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ConsumerKey { get => _ConsumerKey; set { _ConsumerKey = value; OnPropertyChanged(); } }
        public bool IsValidated { get; set; }
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        public string SecretKey { get => _SecretKey; set { _SecretKey = value; OnPropertyChanged(); } }
        public Command SubmitCommand { get; set; }
        public string Token { get => _Token; set { _Token = value; OnPropertyChanged(); } }
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        public void OnPropertyChanged([CallerMemberName]string propName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public void SearchViewModel()
        {
            SubmitCommand = new Command(Submit);
        }

        private void Submit()
        {
        }
    }
}