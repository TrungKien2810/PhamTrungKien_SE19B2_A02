using System;
using System.Linq;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class RoomsWindow : Window
    {
        private readonly RoomService roomService = new RoomService();

        public RoomsWindow()
        {
            InitializeComponent();
            LoadRooms();
        }

        private void LoadRooms()
        {
            try
            {
                var rooms = roomService.GetAll()
                    .OrderBy(r => r.RoomNumber)
                    .ToList();
                
                dgRooms.ItemsSource = rooms;
                txtTotalRooms.Text = $"Total rooms: {rooms.Count}";
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
                var roomEditWindow = new RoomEditWindow();
                if (roomEditWindow.ShowDialog() == true)
                {
                    LoadRooms();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditRoom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as System.Windows.Controls.Button;
                var selectedRoom = button?.Tag as RoomInformation;
                if (selectedRoom == null)
                {
                    MessageBox.Show("Vui lòng chọn một phòng để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var roomEditWindow = new RoomEditWindow(selectedRoom);
                if (roomEditWindow.ShowDialog() == true)
                {
                    LoadRooms();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as System.Windows.Controls.Button;
                var selectedRoom = button?.Tag as RoomInformation;
                if (selectedRoom == null)
                {
                    MessageBox.Show("Vui lòng chọn một phòng để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa phòng {selectedRoom.RoomNumber}?", 
                                           "Xác nhận xóa", 
                                           MessageBoxButton.YesNo, 
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = await roomService.DeleteAsync(selectedRoom.RoomID);
                    if (success)
                    {
                        LoadRooms();
                        MessageBox.Show("Xóa phòng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadRooms();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

