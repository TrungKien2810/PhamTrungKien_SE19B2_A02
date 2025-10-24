using System;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class CustomerRegisterWindow : Window
    {
        private readonly CustomerService customerService = new CustomerService();

        public CustomerRegisterWindow()
        {
            InitializeComponent();
            dpBirthday.SelectedDate = DateTime.Now.AddYears(-25); // Default to 25 years old
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                // Check if email already exists
                var existingCustomer = customerService.GetByEmail(txtEmail.Text.Trim());
                if (existingCustomer != null)
                {
                    MessageBox.Show("Email address already exists. Please use a different email.", 
                                  "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create new customer
                var newCustomer = new Customer
                {
                    CustomerFullName = txtFullName.Text.Trim(),
                    EmailAddress = txtEmail.Text.Trim(),
                    Telephone = txtPhone.Text.Trim(),
                    CustomerBirthday = dpBirthday.SelectedDate,
                    Password = txtPassword.Password,
                    CustomerStatus = 1
                };

                bool success = await customerService.AddAsync(newCustomer);
                if (success)
                {
                    MessageBox.Show("Registration successful! You can now login with your credentials.", 
                                  "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Registration failed. Please try again.", 
                                  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration error: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new CustomerLoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please enter your full name.", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please enter your phone number.", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            if (dpBirthday.SelectedDate == null)
            {
                MessageBox.Show("Please select your birthday.", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                dpBirthday.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password) || txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

