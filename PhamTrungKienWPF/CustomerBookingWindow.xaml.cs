using System;
using System.Linq;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class CustomerBookingWindow : Window
    {
        private readonly RoomService roomService = new RoomService();
        private readonly BookingService bookingService = new BookingService();
        private Customer currentCustomer;

        public CustomerBookingWindow(Customer customer)
        {
            InitializeComponent();
            currentCustomer = customer;
            LoadRoomTypes();
            LoadAvailableRooms();
        }

        private void LoadRoomTypes()
        {
            try
            {
                var roomTypes = roomService.GetAllRoomTypes().ToList();
                
                // Create a new list with "All Types" option
                var roomTypeList = new List<RoomType>();
                roomTypeList.Add(new RoomType { RoomTypeID = 0, RoomTypeName = "All Types" });
                roomTypeList.AddRange(roomTypes);
                
                cbRoomType.ItemsSource = roomTypeList;
                cbRoomType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading room types: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAvailableRooms()
        {
            try
            {
                var rooms = roomService.GetAll().ToList();
                dgAvailableRooms.ItemsSource = rooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rooms: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchTerm = txtSearch.Text.Trim();
                var selectedRoomTypeId = (int)(cbRoomType.SelectedValue ?? 0);

                var rooms = roomService.GetAll().AsQueryable();

                // Filter by search term
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    rooms = rooms.Where(r => 
                        (r.RoomNumber != null && r.RoomNumber.Contains(searchTerm)) ||
                        (r.RoomDetailDescription != null && r.RoomDetailDescription.Contains(searchTerm))
                    );
                }

                // Filter by room type
                if (selectedRoomTypeId > 0)
                {
                    rooms = rooms.Where(r => r.RoomTypeID == selectedRoomTypeId);
                }

                dgAvailableRooms.ItemsSource = rooms.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching rooms: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BookRoom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as System.Windows.Controls.Button;
                var selectedRoom = button?.Tag as RoomInformation;
                if (selectedRoom == null)
                {
                    MessageBox.Show("Please select a room to book.", "Information", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var bookingDetailsWindow = new CustomerBookingDetailsWindow(currentCustomer, selectedRoom);
                if (bookingDetailsWindow.ShowDialog() == true)
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error booking room: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

