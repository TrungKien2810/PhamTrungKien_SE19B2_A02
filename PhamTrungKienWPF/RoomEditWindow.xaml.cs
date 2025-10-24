using System;
using System.Linq;
using System.Windows;
using PhamTrungKienBLL;
using PhamTrungKienModels;

namespace PhamTrungKienWPF
{
    public partial class RoomEditWindow : Window
    {
        private readonly RoomService roomService = new RoomService();
        private RoomInformation? room;
        private bool isEdit;

        public RoomEditWindow(RoomInformation? room = null)
        {
            InitializeComponent();
            this.room = room;
            this.isEdit = room != null;
            
            Loaded += (s, e) => 
            {
                // Test database connection
                if (!roomService.TestConnection())
                {
                    MessageBox.Show("Kết nối database thất bại. Vui lòng kiểm tra kết nối và thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            
            LoadRoomTypes();
            LoadRoomData();
        }

        private void LoadRoomTypes()
        {
            try
            {
                var roomTypes = roomService.GetAllRoomTypes().ToList();
                cbRoomType.ItemsSource = roomTypes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách loại phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRoomData()
        {
            if (isEdit && room != null)
            {
                txtRoomNumber.Text = room.RoomNumber;
                cbRoomType.SelectedValue = room.RoomTypeID;
                txtRoomDescription.Text = room.RoomDetailDescription ?? "";
                txtMaxCapacity.Text = room.RoomMaxCapacity?.ToString() ?? "";
                txtPricePerDay.Text = room.RoomPricePerDay?.ToString() ?? "";
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    var roomToSave = isEdit ? room : new RoomInformation();
                    
                    if (roomToSave != null)
                    {
                        roomToSave.RoomNumber = txtRoomNumber.Text.Trim();
                        roomToSave.RoomTypeID = (int)cbRoomType.SelectedValue;
                        roomToSave.RoomDetailDescription = txtRoomDescription.Text.Trim();
                        roomToSave.RoomMaxCapacity = int.TryParse(txtMaxCapacity.Text, out int capacity) ? capacity : null;
                        roomToSave.RoomPricePerDay = decimal.TryParse(txtPricePerDay.Text, out decimal price) ? price : null;

                        bool success;
                        if (isEdit)
                        {
                            success = await roomService.UpdateAsync(roomToSave);
                            if (success)
                                MessageBox.Show("Cập nhật phòng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            else
                                MessageBox.Show("Không thể cập nhật phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            success = await roomService.AddAsync(roomToSave);
                            if (success)
                                MessageBox.Show("Thêm phòng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            else
                                MessageBox.Show("Không thể thêm phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                MessageBox.Show("Vui lòng nhập số phòng.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRoomNumber.Focus();
                return false;
            }

            if (cbRoomType.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại phòng.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbRoomType.Focus();
                return false;
            }

            // Check if room number already exists (for new rooms or when changing room number)
            if (!isEdit || (isEdit && room != null && txtRoomNumber.Text.Trim() != room.RoomNumber))
            {
                var existingRoom = roomService.GetByRoomNumber(txtRoomNumber.Text.Trim());
                if (existingRoom != null)
                {
                    MessageBox.Show("Số phòng đã tồn tại. Vui lòng chọn số phòng khác.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtRoomNumber.Focus();
                    return false;
                }
            }

            int capacity = 0;
            if (!string.IsNullOrWhiteSpace(txtMaxCapacity.Text))
            {
                if (!int.TryParse(txtMaxCapacity.Text, out capacity))
                {
                    MessageBox.Show("Sức chứa phải là số nguyên.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtMaxCapacity.Focus();
                    return false;
                }

                if (capacity <= 0)
                {
                    MessageBox.Show("Sức chứa phải lớn hơn 0.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtMaxCapacity.Focus();
                    return false;
                }
            }

            decimal price = 0;
            if (!string.IsNullOrWhiteSpace(txtPricePerDay.Text))
            {
                if (!decimal.TryParse(txtPricePerDay.Text, out price))
                {
                    MessageBox.Show("Giá phòng phải là số.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPricePerDay.Focus();
                    return false;
                }

                if (price < 0)
                {
                    MessageBox.Show("Giá phòng không được âm.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPricePerDay.Focus();
                    return false;
                }
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}