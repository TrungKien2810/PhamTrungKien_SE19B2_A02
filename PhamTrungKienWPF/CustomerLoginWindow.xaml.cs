using System;
using System.Windows;
using PhamTrungKienBLL;

namespace PhamTrungKienWPF
{
    public partial class CustomerLoginWindow : Window
    {
        private readonly AuthService authService = new AuthService();

        public CustomerLoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Password;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both email and password.", "Validation Error", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var customer = authService.Authenticate(email, password);
                if (customer != null)
                {
                    // Check if it's admin account
                    if (customer.CustomerID == 0) // Admin account
                    {
                        MessageBox.Show("Please use Admin Login for administrator access.", 
                                      "Access Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // Open customer dashboard
                    var customerDashboard = new CustomerDashboardWindow(customer);
                    customerDashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid email or password. Please try again.", 
                                  "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new CustomerRegisterWindow();
            if (registerWindow.ShowDialog() == true)
            {
                MessageBox.Show("Registration successful! Please login with your credentials.", 
                              "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

