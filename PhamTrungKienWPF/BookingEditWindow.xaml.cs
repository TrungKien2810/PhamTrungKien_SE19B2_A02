using System;
using System.Linq;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class BookingEditWindow : Window
    {
        private readonly BookingService bookingService = new BookingService();
        private readonly CustomerService customerService = new CustomerService();
        private BookingReservation? booking;
        private bool isEdit;

        public BookingEditWindow(BookingReservation? booking = null)
        {
            InitializeComponent();
            this.booking = booking;
            this.isEdit = booking != null;
            
            LoadCustomers();
            LoadBookingData();
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = customerService.GetAll().ToList();
                cbCustomer.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadBookingData()
        {
            if (isEdit && booking != null)
            {
                cbCustomer.SelectedValue = booking.CustomerID;
                dpBookingDate.SelectedDate = booking.BookingDate;
                txtTotalPrice.Text = booking.TotalPrice?.ToString() ?? "";
                cbBookingStatus.SelectedIndex = (booking.BookingStatus ?? 1) - 1;
            }
            else
            {
                dpBookingDate.SelectedDate = DateTime.Now;
                cbBookingStatus.SelectedIndex = 0; // Default to "Đã xác nhận"
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    var bookingToSave = isEdit ? booking : new BookingReservation();
                    
                    if (bookingToSave != null)
                    {
                        bookingToSave.CustomerID = (int)cbCustomer.SelectedValue;
                        bookingToSave.BookingDate = dpBookingDate.SelectedDate;
                        bookingToSave.TotalPrice = decimal.Parse(txtTotalPrice.Text);
                        // Fix casting issue - convert string to byte
                        if (cbBookingStatus.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem && selectedItem.Tag is string tagValue)
                        {
                            if (byte.TryParse(tagValue, out byte statusValue))
                            {
                                bookingToSave.BookingStatus = statusValue;
                            }
                            else
                            {
                                bookingToSave.BookingStatus = 1; // Default to confirmed
                            }
                        }
                        else
                        {
                            bookingToSave.BookingStatus = 1; // Default to confirmed
                        }

                        bool success;
                        if (isEdit)
                        {
                            success = await bookingService.UpdateAsync(bookingToSave);
                            if (success)
                                MessageBox.Show("Cập nhật đặt phòng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            else
                                MessageBox.Show("Không thể cập nhật đặt phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            success = await bookingService.AddAsync(bookingToSave);
                            if (success)
                                MessageBox.Show("Thêm đặt phòng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            else
                                MessageBox.Show("Không thể thêm đặt phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (cbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (dpBookingDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày đặt.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTotalPrice.Text) || !decimal.TryParse(txtTotalPrice.Text, out _))
            {
                MessageBox.Show("Vui lòng nhập tổng tiền hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}