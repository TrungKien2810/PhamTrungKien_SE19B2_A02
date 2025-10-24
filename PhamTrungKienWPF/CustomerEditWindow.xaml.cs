using System;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class CustomerEditWindow : Window
    {
        private readonly CustomerService customerService;
        private Customer? customer;
        private bool isEdit;

        public CustomerEditWindow()
        {
            InitializeComponent();
            customerService = new CustomerService();
            isEdit = false;
            Loaded += (s, e) => 
            {
                // Test database connection
                if (!customerService.TestConnection())
                {
                    ShowError("Database connection failed. Please check your connection and try again.");
                }
                FullNameTextBox.Focus();
            };
        }

        public CustomerEditWindow(Customer customer) : this()
        {
            this.customer = customer;
            isEdit = true;
            HeaderText.Text = "Edit Customer";
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            if (customer != null)
            {
                FullNameTextBox.Text = customer.CustomerFullName;
                EmailTextBox.Text = customer.EmailAddress;
                TelephoneTextBox.Text = customer.Telephone;
                BirthdayDatePicker.SelectedDate = customer.CustomerBirthday;
                PasswordBox.Password = customer.Password;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    var customerToSave = isEdit ? customer : new Customer();
                    
                    if (customerToSave != null)
                    {
                        customerToSave.CustomerFullName = FullNameTextBox.Text.Trim();
                        customerToSave.EmailAddress = EmailTextBox.Text.Trim();
                        customerToSave.Telephone = TelephoneTextBox.Text.Trim();
                        customerToSave.CustomerBirthday = BirthdayDatePicker.SelectedDate;
                        customerToSave.Password = PasswordBox.Password;

                        bool success;
                        if (isEdit)
                        {
                            success = await customerService.UpdateAsync(customerToSave);
                            if (success)
                                MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            else
                                MessageBox.Show("Failed to update customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            // Check if email already exists before adding (includes inactive customers)
                            var emailExists = customerService.EmailExists(customerToSave.EmailAddress);
                            if (emailExists)
                            {
                                ShowError("Email address already exists. Please use a different email.");
                                EmailTextBox.Focus();
                                return;
                            }

                            success = await customerService.AddAsync(customerToSave);
                            if (success)
                            {
                                MessageBox.Show("Customer added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                ShowError("Failed to add customer. Please check your input and try again.");
                                return;
                            }
                        }

                        if (success)
                        {
                            DialogResult = true;
                            Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                ShowError("Full name is required.");
                FullNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ShowError("Email is required.");
                EmailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(EmailTextBox.Text.Trim()))
            {
                ShowError("Please enter a valid email address.");
                EmailTextBox.Focus();
                return false;
            }

            // Telephone is optional; validate format only if provided
            if (!string.IsNullOrWhiteSpace(TelephoneTextBox.Text) && !IsValidPhoneNumber(TelephoneTextBox.Text.Trim()))
            {
                ShowError("Please enter a valid phone number.");
                TelephoneTextBox.Focus();
                return false;
            }

            // Birthday is optional; if provided, ensure not in the future
            if (BirthdayDatePicker.SelectedDate != null && BirthdayDatePicker.SelectedDate > DateTime.Now)
            {
                ShowError("Birthday cannot be in the future.");
                BirthdayDatePicker.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ShowError("Password is required.");
                PasswordBox.Focus();
                return false;
            }

            if (PasswordBox.Password.Length < 6)
            {
                ShowError("Password must be at least 6 characters long.");
                PasswordBox.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            try
            {
                var regex = new System.Text.RegularExpressions.Regex(@"^[0-9+\-\s()]+$");
                return regex.IsMatch(phoneNumber) && phoneNumber.Length >= 10;
            }
            catch
            {
                return false;
            }
        }

        private void ShowError(string message)
        {
            ErrorMessageText.Text = message;
            ErrorMessageText.Visibility = Visibility.Visible;
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

