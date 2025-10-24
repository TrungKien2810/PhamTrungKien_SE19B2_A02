using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class CustomersWindow : Window
    {
        private readonly CustomerService customerService;
        private List<Customer> customers;

        public CustomersWindow()
        {
            InitializeComponent();
            customerService = new CustomerService();
            customers = new List<Customer>();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                customers = customerService.GetAll().ToList();
                CustomerDataGrid.ItemsSource = customers;
                StatusText.Text = $"Total customers: {customers.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string searchTerm = SearchTextBox.Text.Trim();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    CustomerDataGrid.ItemsSource = customers;
                }
                else
                {
                    var filteredCustomers = customers.Where(c => 
                        (!string.IsNullOrEmpty(c.CustomerFullName) && c.CustomerFullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(c.EmailAddress) && c.EmailAddress.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(c.Telephone) && c.Telephone.Contains(searchTerm))).ToList();
                    CustomerDataGrid.ItemsSource = filteredCustomers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new CustomerEditWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadCustomers();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var customer = (Customer)((Button)sender).DataContext;
            var editWindow = new CustomerEditWindow(customer);
            if (editWindow.ShowDialog() == true)
            {
                LoadCustomers();
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var customer = (Customer)((Button)sender).DataContext;
            
            var result = MessageBox.Show($"Are you sure you want to delete customer {customer.CustomerFullName}?", 
                                       "Confirm Delete", 
                                       MessageBoxButton.YesNo, 
                                       MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = await customerService.DeleteAsync(customer.CustomerID);
                    if (success)
                    {
                        LoadCustomers();
                        MessageBox.Show("Customer deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomers();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

