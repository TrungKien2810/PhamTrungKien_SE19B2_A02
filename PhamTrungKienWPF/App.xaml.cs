using System.Windows;

namespace PhamTrungKienWPF
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Show login window on startup
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}

