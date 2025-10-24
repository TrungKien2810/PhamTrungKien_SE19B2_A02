using System;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService authService;

        public LoginWindow()
        {
            InitializeComponent();
            authService = new AuthService();
            
            // Set focus to email textbox
            Loaded += (s, e) => EmailTextBox.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = EmailTextBox.Text.Trim();
                string password = PasswordBox.Password;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ShowError("Please enter both email and password.");
                    return;
                }

                var customer = authService.Authenticate(email, password);
                if (customer != null)
                {
                    if (rbAdmin.IsChecked == true)
                    {
                        // Check if it's admin account
                        if (customer.CustomerID == 0) // Admin account
                        {
                            // Open admin main window
                            var mainWindow = new MainWindow();
                            mainWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            ShowError("This is not an administrator account. Please select 'Customer' login type.");
                        }
                    }
                    else if (rbCustomer.IsChecked == true)
                    {
                        // Check if it's customer account
                        if (customer.CustomerID != 0) // Customer account
                        {
                            // Open customer dashboard
                            var customerDashboard = new CustomerDashboardWindow(customer);
                            customerDashboard.Show();
                            this.Close();
                        }
                        else
                        {
                            ShowError("This is not a customer account. Please select 'Administrator' login type.");
                        }
                    }
                }
                else
                {
                    ShowError("Invalid email or password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Login error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            ErrorMessageText.Text = message;
            ErrorMessageText.Visibility = Visibility.Visible;
        }
    }
}

