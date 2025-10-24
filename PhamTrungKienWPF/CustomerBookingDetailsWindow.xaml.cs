using System;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class CustomerBookingDetailsWindow : Window
    {
        private readonly BookingService bookingService = new BookingService();
        private Customer currentCustomer;
        private RoomInformation selectedRoom;

        public CustomerBookingDetailsWindow(Customer customer, RoomInformation room)
        {
            InitializeComponent();
            currentCustomer = customer;
            selectedRoom = room;
            InitializeBookingForm();
        }

        private void InitializeBookingForm()
        {
            txtRoomInfo.Text = $"{selectedRoom.RoomNumber} - {selectedRoom.RoomType?.RoomTypeName}";
            txtPricePerDay.Text = selectedRoom.RoomPricePerDay?.ToString("C0") ?? "N/A";
            
            dpCheckIn.SelectedDate = DateTime.Today;
            dpCheckOut.SelectedDate = DateTime.Today.AddDays(1);
            
            dpCheckIn.SelectedDateChanged += OnDateChanged;
            dpCheckOut.SelectedDateChanged += OnDateChanged;
            
            CalculateTotalPrice();
        }

        private void OnDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            if (dpCheckIn.SelectedDate.HasValue && dpCheckOut.SelectedDate.HasValue)
            {
                var checkIn = dpCheckIn.SelectedDate.Value;
                var checkOut = dpCheckOut.SelectedDate.Value;
                
                if (checkOut > checkIn)
                {
                    var numberOfDays = (checkOut - checkIn).Days;
                    txtNumberOfDays.Text = numberOfDays.ToString();
                    
                    if (selectedRoom.RoomPricePerDay.HasValue)
                    {
                        var totalPrice = numberOfDays * selectedRoom.RoomPricePerDay.Value;
                        txtTotalPrice.Text = totalPrice.ToString("C0");
                    }
                    else
                    {
                        txtTotalPrice.Text = "N/A";
                    }
                }
                else
                {
                    txtNumberOfDays.Text = "0";
                    txtTotalPrice.Text = "Ngày không hợp lệ";
                }
            }
        }

        private async void ConfirmBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                // At this point, ValidateInput() ensured SelectedDate has value
                var checkIn = dpCheckIn.SelectedDate!.Value;
                var checkOut = dpCheckOut.SelectedDate!.Value;
                var numberOfDays = (checkOut - checkIn).Days;
                var totalPrice = numberOfDays * (selectedRoom.RoomPricePerDay ?? 0);

                // Create booking reservation
                var booking = new BookingReservation
                {
                    CustomerID = currentCustomer.CustomerID,
                    BookingDate = DateTime.Now,
                    TotalPrice = totalPrice,
                    BookingStatus = 1 // Confirmed
                };

                // Add booking first
                bool bookingSuccess = await bookingService.AddAsync(booking);
                if (!bookingSuccess)
                {
                    MessageBox.Show("Không thể tạo đặt phòng. Vui lòng thử lại.", "Lỗi", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create booking detail using the assigned ID on booking
                var bookingDetail = new BookingDetail
                {
                    BookingReservationID = booking.BookingReservationID,
                    RoomID = selectedRoom.RoomID,
                    StartDate = checkIn,
                    EndDate = checkOut,
                    ActualPrice = selectedRoom.RoomPricePerDay
                };

                // Add booking detail
                bool detailSuccess = await bookingService.AddBookingDetailAsync(bookingDetail);
                if (detailSuccess)
                {
                    MessageBox.Show("Đặt phòng thành công!", "Thành công", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Không thể tạo chi tiết đặt phòng.", "Lỗi", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt phòng: {ex.Message}", "Lỗi", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (!dpCheckIn.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày nhận phòng.", "Lỗi xác thực", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                dpCheckIn.Focus();
                return false;
            }

            if (!dpCheckOut.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày trả phòng.", "Lỗi xác thực", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                dpCheckOut.Focus();
                return false;
            }

            if (dpCheckOut.SelectedDate.HasValue && dpCheckIn.SelectedDate.HasValue && dpCheckOut.SelectedDate.Value <= dpCheckIn.SelectedDate.Value)
            {
                MessageBox.Show("Ngày trả phòng phải sau ngày nhận phòng.", "Lỗi xác thực", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                dpCheckOut.Focus();
                return false;
            }

            if (dpCheckIn.SelectedDate.HasValue && dpCheckIn.SelectedDate.Value < DateTime.Today)
            {
                MessageBox.Show("Ngày nhận phòng không thể trong quá khứ.", "Lỗi xác thực", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                dpCheckIn.Focus();
                return false;
            }

            return true;
        }
    }
}