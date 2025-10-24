using System;
using System.Linq;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class BookingsWindow : Window
    {
        private readonly BookingService bookingService = new BookingService();
        private readonly CustomerService customerService = new CustomerService();

        public BookingsWindow()
        {
            InitializeComponent();
            LoadBookings();
        }

        private void LoadBookings()
        {
            try
            {
                var bookings = bookingService.GetAll()
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();
                
                dgBookings.ItemsSource = bookings;
                txtTotalBookings.Text = $"Tổng số đặt phòng: {bookings.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var bookingEditWindow = new BookingEditWindow();
                if (bookingEditWindow.ShowDialog() == true)
                {
                    LoadBookings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm đặt phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as System.Windows.Controls.Button;
                var selectedBooking = button?.Tag as BookingReservation;
                if (selectedBooking == null)
                {
                    MessageBox.Show("Vui lòng chọn một đặt phòng để xem chi tiết.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Always fetch a fresh, fully-loaded booking for details view
                var refreshed = bookingService.GetById(selectedBooking.BookingReservationID) ?? selectedBooking;
                var detailsWindow = new BookingDetailsWindow(refreshed);
                detailsWindow.Owner = this;
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteBooking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as System.Windows.Controls.Button;
                var selectedBooking = button?.Tag as BookingReservation;
                if (selectedBooking == null)
                {
                    MessageBox.Show("Vui lòng chọn một đặt phòng để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa đặt phòng #{selectedBooking.BookingReservationID}?", 
                                           "Xác nhận xóa", 
                                           MessageBoxButton.YesNo, 
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = await bookingService.DeleteAsync(selectedBooking.BookingReservationID);
                    if (success)
                    {
                        LoadBookings();
                        MessageBox.Show("Xóa đặt phòng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa đặt phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa đặt phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBookings();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}