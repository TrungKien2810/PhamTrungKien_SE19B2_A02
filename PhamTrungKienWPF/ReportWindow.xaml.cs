using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PhamTrungKienBLL;

namespace PhamTrungKienWPF
{
    public partial class ReportWindow : Window
    {
        private readonly ReportService reportService = new ReportService();

        public ReportWindow()
        {
            InitializeComponent();
            InitializeDatePickers();
            // Trigger initial report generation with valid arguments to avoid nullability warnings
            GenerateReport_Click(this, new RoutedEventArgs());
        }

        private void InitializeDatePickers()
        {
            dpFromDate.SelectedDate = DateTime.Now.AddMonths(-1);
            dpToDate.SelectedDate = DateTime.Now;
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fromDate = dpFromDate.SelectedDate ?? DateTime.Now.AddMonths(-1);
                var toDate = dpToDate.SelectedDate ?? DateTime.Now;

                if (fromDate > toDate)
                {
                    MessageBox.Show("From date cannot be greater than To date.", "Invalid Date Range", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Always load summary data
                LoadSummaryReport(fromDate, toDate);

                // Load specific report based on selection
                if (rbSummary.IsChecked == true)
                {
                    LoadSummaryReport(fromDate, toDate);
                }
                else if (rbRevenueByRoom.IsChecked == true)
                {
                    LoadRevenueByRoomReport(fromDate, toDate);
                }
                else if (rbRevenueByRoomType.IsChecked == true)
                {
                    LoadRevenueByRoomTypeReport(fromDate, toDate);
                }
                else if (rbCustomerReport.IsChecked == true)
                {
                    LoadCustomerReport(fromDate, toDate);
                }
                else if (rbRoomOccupancy.IsChecked == true)
                {
                    LoadRoomOccupancyReport(fromDate, toDate);
                }

                txtReportInfo.Text = $"Report generated for period: {fromDate:dd/MM/yyyy} - {toDate:dd/MM/yyyy}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSummaryReport(DateTime fromDate, DateTime toDate)
        {
            var summary = reportService.GetSummaryReport(fromDate, toDate);
            
            txtTotalRevenue.Text = summary.TotalRevenue.ToString("C0");
            txtTotalBookings.Text = summary.TotalBookings.ToString();
            txtTotalCustomers.Text = summary.TotalCustomers.ToString();
            txtTotalRooms.Text = summary.TotalRooms.ToString();

            // Hide summary cards for other reports
            SummaryCards.Visibility = Visibility.Visible;
            dgReport.Visibility = Visibility.Collapsed;
        }

        private void LoadRevenueByRoomReport(DateTime fromDate, DateTime toDate)
        {
            var data = reportService.GetRevenueByRoom(fromDate, toDate).ToList();
            
            dgReport.ItemsSource = data;
            dgReport.Columns.Clear();
            
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Room Number", Binding = new System.Windows.Data.Binding("RoomNumber"), Width = 120 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Room Type", Binding = new System.Windows.Data.Binding("RoomType"), Width = 150 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Check-in", Binding = new System.Windows.Data.Binding("CheckIn") { StringFormat = "dd/MM/yyyy" }, Width = 100 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Check-out", Binding = new System.Windows.Data.Binding("CheckOut") { StringFormat = "dd/MM/yyyy" }, Width = 100 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Revenue", Binding = new System.Windows.Data.Binding("Revenue") { StringFormat = "C0" }, Width = 120 });

            SummaryCards.Visibility = Visibility.Collapsed;
            dgReport.Visibility = Visibility.Visible;
        }

        private void LoadRevenueByRoomTypeReport(DateTime fromDate, DateTime toDate)
        {
            var data = reportService.GetRevenueByRoomType(fromDate, toDate).ToList();
            
            dgReport.ItemsSource = data;
            dgReport.Columns.Clear();
            
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Room Type", Binding = new System.Windows.Data.Binding("RoomTypeName"), Width = 200 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Total Revenue", Binding = new System.Windows.Data.Binding("TotalRevenue") { StringFormat = "C0" }, Width = 150 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Booking Count", Binding = new System.Windows.Data.Binding("BookingCount"), Width = 120 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Avg Revenue/Booking", Binding = new System.Windows.Data.Binding("TotalRevenue") { StringFormat = "C0" }, Width = 150 });

            SummaryCards.Visibility = Visibility.Collapsed;
            dgReport.Visibility = Visibility.Visible;
        }

        private void LoadCustomerReport(DateTime fromDate, DateTime toDate)
        {
            var data = reportService.GetCustomerReport(fromDate, toDate).ToList();
            
            dgReport.ItemsSource = data;
            dgReport.Columns.Clear();
            
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Customer Name", Binding = new System.Windows.Data.Binding("CustomerName"), Width = 150 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new System.Windows.Data.Binding("Email"), Width = 200 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Phone", Binding = new System.Windows.Data.Binding("Phone"), Width = 120 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Total Bookings", Binding = new System.Windows.Data.Binding("TotalBookings"), Width = 120 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Total Spent", Binding = new System.Windows.Data.Binding("TotalSpent") { StringFormat = "C0" }, Width = 120 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Last Booking", Binding = new System.Windows.Data.Binding("LastBookingDate") { StringFormat = "dd/MM/yyyy" }, Width = 120 });

            SummaryCards.Visibility = Visibility.Collapsed;
            dgReport.Visibility = Visibility.Visible;
        }

        private void LoadRoomOccupancyReport(DateTime fromDate, DateTime toDate)
        {
            var data = reportService.GetRoomOccupancyReport(fromDate, toDate).ToList();
            
            dgReport.ItemsSource = data;
            dgReport.Columns.Clear();
            
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Room Number", Binding = new System.Windows.Data.Binding("RoomNumber"), Width = 120 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Room Type", Binding = new System.Windows.Data.Binding("RoomType"), Width = 150 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Total Days", Binding = new System.Windows.Data.Binding("TotalDays"), Width = 100 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Occupied Days", Binding = new System.Windows.Data.Binding("OccupiedDays"), Width = 120 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Occupancy Rate", Binding = new System.Windows.Data.Binding("OccupancyRate") { StringFormat = "F1" }, Width = 120 });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Total Revenue", Binding = new System.Windows.Data.Binding("TotalRevenue") { StringFormat = "C0" }, Width = 120 });

            SummaryCards.Visibility = Visibility.Collapsed;
            dgReport.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

