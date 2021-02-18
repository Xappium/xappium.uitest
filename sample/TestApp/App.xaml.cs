using TestApp.Pages;
using Xamarin.Forms;

namespace TestApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new LoginPage();
        }
    }
}
