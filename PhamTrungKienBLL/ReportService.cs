using System;
using System.Collections.Generic;
using System.Linq;
using PhamTrungKienDAL;
using PhamTrungKienModels;

namespace PhamTrungKienBLL
{
    public class ReportService
    {
        private readonly DatabaseService db = DatabaseService.Instance;

        // Revenue Reports
        public List<RevenueReport> GetRevenueByRoom(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var bookingDetails = db.BookingDetails
                    .Where(bd => bd.StartDate >= fromDate && bd.EndDate <= toDate)
                    .Join(db.RoomInformations, bd => bd.RoomID, r => r.RoomID, (bd, room) => new { bd, room })
                    .Join(db.RoomTypes, br => br.room.RoomTypeID, rt => rt.RoomTypeID, (br, roomType) => new
                    {
                        RoomID = br.room.RoomID,
                        RoomNumber = br.room.RoomNumber,
                        RoomType = roomType.RoomTypeName,
                        CheckIn = br.bd.StartDate,
                        CheckOut = br.bd.EndDate,
                        Revenue = br.bd.ActualPrice ?? 0
                    })
                    .ToList();

                return bookingDetails.Select(x => new RevenueReport
                {
                    RoomID = x.RoomID,
                    RoomNumber = x.RoomNumber,
                    RoomType = x.RoomType,
                    CheckIn = x.CheckIn,
                    CheckOut = x.CheckOut,
                    Revenue = x.Revenue
                }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetRevenueByRoom: {ex.Message}");
                return new List<RevenueReport>();
            }
        }

        public List<RevenueByRoomType> GetRevenueByRoomType(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var revenue = db.BookingDetails
                    .Where(bd => bd.StartDate >= fromDate && bd.EndDate <= toDate)
                    .Join(db.RoomInformations, bd => bd.RoomID, r => r.RoomID, (bd, room) => new { bd, room })
                    .Join(db.RoomTypes, br => br.room.RoomTypeID, rt => rt.RoomTypeID, (br, roomType) => new
                    {
                        RoomTypeID = roomType.RoomTypeID,
                        RoomTypeName = roomType.RoomTypeName,
                        Revenue = br.bd.ActualPrice ?? 0
                    })
                    .GroupBy(x => new { x.RoomTypeID, x.RoomTypeName })
                    .Select(g => new RevenueByRoomType
                    {
                        RoomTypeID = g.Key.RoomTypeID,
                        RoomTypeName = g.Key.RoomTypeName,
                        TotalRevenue = g.Sum(x => x.Revenue),
                        BookingCount = g.Count()
                    })
                    .OrderByDescending(x => x.TotalRevenue)
                    .ToList();

                return revenue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetRevenueByRoomType: {ex.Message}");
                return new List<RevenueByRoomType>();
            }
        }

        // Customer Reports
        public List<CustomerReport> GetCustomerReport(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var customers = db.Customers
                    .Where(c => c.CustomerStatus == 1)
                    .Select(c => new CustomerReport
                    {
                        CustomerID = c.CustomerID,
                        CustomerName = c.CustomerFullName ?? "N/A",
                        Email = c.EmailAddress,
                        Phone = c.Telephone ?? "N/A",
                        TotalBookings = c.BookingReservations.Count(br => 
                            br.BookingDate >= fromDate && br.BookingDate <= toDate),
                        TotalSpent = c.BookingReservations
                            .Where(br => br.BookingDate >= fromDate && br.BookingDate <= toDate)
                            .Sum(br => br.TotalPrice ?? 0),
                        LastBookingDate = c.BookingReservations
                            .Where(br => br.BookingDate >= fromDate && br.BookingDate <= toDate)
                            .Max(br => br.BookingDate)
                    })
                    .OrderByDescending(c => c.TotalSpent)
                    .ToList();

                return customers;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetCustomerReport: {ex.Message}");
                return new List<CustomerReport>();
            }
        }

        // Room Reports
        public List<RoomOccupancyReport> GetRoomOccupancyReport(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var rooms = db.RoomInformations
                    .Where(r => r.RoomStatus == 1)
                    .Select(r => new RoomOccupancyReport
                    {
                        RoomID = r.RoomID,
                        RoomNumber = r.RoomNumber,
                        RoomType = r.RoomType.RoomTypeName,
                        TotalDays = (toDate - fromDate).Days + 1,
                        OccupiedDays = 0, // Will be calculated
                        OccupancyRate = 0, // Will be calculated
                        TotalRevenue = r.BookingDetails
                            .Where(bd => bd.StartDate >= fromDate && bd.EndDate <= toDate)
                            .Sum(bd => bd.ActualPrice ?? 0)
                    })
                    .ToList();

                // Calculate occupied days and occupancy rate
                foreach (var room in rooms)
                {
                    var occupiedDays = 0;
                    var bookingDetails = db.BookingDetails
                        .Where(bd => bd.RoomID == room.RoomID && bd.StartDate <= toDate && bd.EndDate >= fromDate)
                        .ToList();

                    foreach (var bd in bookingDetails)
                    {
                        var start = bd.StartDate > fromDate ? bd.StartDate : fromDate;
                        var end = bd.EndDate < toDate ? bd.EndDate : toDate;
                        occupiedDays += (end - start).Days + 1;
                    }

                    room.OccupiedDays = occupiedDays;
                    if (room.TotalDays > 0)
                    {
                        room.OccupancyRate = (double)room.OccupiedDays / room.TotalDays * 100;
                    }
                }

                return rooms.OrderByDescending(r => r.OccupancyRate).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetRoomOccupancyReport: {ex.Message}");
                return new List<RoomOccupancyReport>();
            }
        }

        // Summary Statistics
        public SummaryReport GetSummaryReport(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var totalRevenue = db.BookingReservations
                    .Where(br => br.BookingDate >= fromDate && br.BookingDate <= toDate)
                    .Sum(br => br.TotalPrice ?? 0);

                var totalBookings = db.BookingReservations
                    .Count(br => br.BookingDate >= fromDate && br.BookingDate <= toDate);

                var totalCustomers = db.Customers
                    .Count(c => c.CustomerStatus == 1);

                var totalRooms = db.RoomInformations
                    .Count(r => r.RoomStatus == 1);

                var averageBookingValue = totalBookings > 0 ? totalRevenue / totalBookings : 0;

                return new SummaryReport
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    TotalRevenue = totalRevenue,
                    TotalBookings = totalBookings,
                    TotalCustomers = totalCustomers,
                    TotalRooms = totalRooms,
                    AverageBookingValue = averageBookingValue
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetSummaryReport: {ex.Message}");
                return new SummaryReport
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    TotalRevenue = 0,
                    TotalBookings = 0,
                    TotalCustomers = 0,
                    TotalRooms = 0,
                    AverageBookingValue = 0
                };
            }
        }
    }

    // Report Models
    public class RevenueReport
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RevenueByRoomType
    {
        public int RoomTypeID { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int BookingCount { get; set; }
    }

    public class CustomerReport
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastBookingDate { get; set; }
    }

    public class RoomOccupancyReport
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int OccupiedDays { get; set; }
        public double OccupancyRate { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class SummaryReport
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalRooms { get; set; }
        public decimal AverageBookingValue { get; set; }
    }
}

