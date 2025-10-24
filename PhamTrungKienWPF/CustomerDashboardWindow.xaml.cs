using System;
using System.Linq;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class CustomerDashboardWindow : Window
    {
        private readonly CustomerService customerService = new CustomerService();
        private readonly BookingService bookingService = new BookingService();
        private Customer currentCustomer;

        public CustomerDashboardWindow(Customer customer)
        {
            InitializeComponent();
            currentCustomer = customer;
            LoadCustomerProfile();
            LoadBookingHistory();
        }

        private void LoadCustomerProfile()
        {
            txtWelcome.Text = $"Welcome, {currentCustomer.CustomerFullName}!";
            txtProfileFullName.Text = currentCustomer.CustomerFullName ?? "N/A";
            txtProfileEmail.Text = currentCustomer.EmailAddress ?? "N/A";
            txtProfilePhone.Text = currentCustomer.Telephone ?? "N/A";
            txtProfileBirthday.Text = currentCustomer.CustomerBirthday?.ToString("d") ?? "N/A";
            txtProfileStatus.Text = currentCustomer.CustomerStatus == 1 ? "Active" : "Inactive";
        }

        private void LoadBookingHistory()
        {
            try
            {
                var bookings = bookingService.GetAll()
                    .Where(b => b.CustomerID == currentCustomer.CustomerID)
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();

                dgBookings.ItemsSource = bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading booking history: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var editWindow = new CustomerEditWindow(currentCustomer);
                if (editWindow.ShowDialog() == true)
                {
                    // Refresh customer data
                    var updatedCustomer = customerService.GetById(currentCustomer.CustomerID);
                    if (updatedCustomer != null)
                    {
                        currentCustomer = updatedCustomer;
                        LoadCustomerProfile();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing profile: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var bookingWindow = new CustomerBookingWindow(currentCustomer);
                if (bookingWindow.ShowDialog() == true)
                {
                    LoadBookingHistory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating new booking: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshBookingsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBookingHistory();
        }

        private void ViewBookingDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as System.Windows.Controls.Button;
                var selectedBooking = button?.Tag as BookingReservation;
                if (selectedBooking == null)
                {
                    MessageBox.Show("Please select a booking to view details.", "Information", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var detailsWindow = new BookingDetailsWindow(selectedBooking);
                detailsWindow.Owner = this;
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing booking details: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", 
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}

