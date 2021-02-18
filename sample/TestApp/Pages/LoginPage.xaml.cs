using Xamarin.Forms;

namespace TestApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            loginButton.Command = new Command(DoLogin);
        }

        private void DoLogin()
        {
            App.Current.MainPage = new MainPage
            {
                BindingContext = new { Username = usernameEntry.Text }
            };
        }
    }
}
