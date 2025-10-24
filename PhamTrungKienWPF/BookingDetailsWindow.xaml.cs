using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class BookingDetailsWindow : Window
    {
        private readonly BookingService bookingService = new BookingService();
        private BookingReservation booking;

        public BookingDetailsWindow(BookingReservation booking)
        {
            InitializeComponent();
            // Fetch a fresh entity with navigation properties loaded
            var loaded = bookingService.GetById(booking.BookingReservationID);
            this.booking = loaded ?? booking;
            LoadBookingDetails();
        }

        private void LoadBookingDetails()
        {
            try
            {
                // Load booking info
                txtBookingID.Text = booking.BookingReservationID.ToString();
                txtCustomer.Text = booking.Customer?.CustomerFullName ?? "N/A";
                txtBookingDate.Text = booking.BookingDate?.ToString("dd/MM/yyyy") ?? "N/A";
                txtTotalPrice.Text = booking.TotalPrice?.ToString("C0") ?? "N/A";

                // Load booking details from database to ensure we have fresh data
                var details = bookingService.GetBookingDetails(booking.BookingReservationID).ToList();
                
                // Load room information for each detail
                var roomService = new RoomService();
                var roomTypeService = new RoomTypeService();
                foreach (var detail in details)
                {
                    if (detail.RoomInformation == null)
                    {
                        detail.RoomInformation = roomService.GetById(detail.RoomID);
                    }

                    // Ensure RoomType is loaded so bindings like RoomInformation.RoomType.RoomTypeName work
                    if (detail.RoomInformation != null && detail.RoomInformation.RoomType == null)
                    {
                        detail.RoomInformation.RoomType = roomTypeService.GetById(detail.RoomInformation.RoomTypeID);
                    }
                }

                // Fallback: if no details were returned (e.g., stale state), try from booking entity
                if (details.Count == 0 && booking.BookingDetails != null && booking.BookingDetails.Any())
                {
                    details = booking.BookingDetails.ToList();
                }
                dgBookingDetails.ItemsSource = details;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết đặt phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

