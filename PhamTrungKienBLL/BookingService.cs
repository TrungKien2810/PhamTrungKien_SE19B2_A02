using Microsoft.EntityFrameworkCore;
using PhamTrungKienDAL;
using PhamTrungKienModels;

namespace PhamTrungKienBLL
{
    public class BookingService
    {
        private readonly DatabaseService db = DatabaseService.Instance;

        public IEnumerable<BookingReservation> GetAll()
        {
            return db.Context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.RoomInformation)
                        .ThenInclude(r => r.RoomType);
        }

        public BookingReservation? GetById(int id)
        {
            return db.Context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.RoomInformation)
                        .ThenInclude(r => r.RoomType)
                .FirstOrDefault(b => b.BookingReservationID == id);
        }

        public async Task<bool> AddAsync(BookingReservation booking)
        {
            try
            {
                // Generate new BookingReservationID only if not set
                if (booking.BookingReservationID == 0)
                {
                    var maxId = db.BookingReservations.Any() ? db.BookingReservations.Max(x => x.BookingReservationID) : 0;
                    booking.BookingReservationID = maxId + 1;
                }
                
                // Set default values
                if (booking.BookingDate == null)
                    booking.BookingDate = DateTime.Now;
                if (booking.BookingStatus == null)
                    booking.BookingStatus = 1; // Confirmed
                
                db.Context.BookingReservations.Add(booking);
                await db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(BookingReservation booking)
        {
            try
            {
                var existing = await db.Context.BookingReservations.FindAsync(booking.BookingReservationID);
                if (existing == null) return false;
                
                existing.BookingDate = booking.BookingDate;
                existing.TotalPrice = booking.TotalPrice;
                existing.CustomerID = booking.CustomerID;
                existing.BookingStatus = booking.BookingStatus;
                
                await db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var booking = await db.Context.BookingReservations.FindAsync(id);
                if (booking == null) return false;
                
                // First delete all booking details
                var details = db.Context.BookingDetails.Where(bd => bd.BookingReservationID == id).ToList();
                foreach (var detail in details)
                {
                    db.Context.BookingDetails.Remove(detail);
                }
                
                // Then delete the booking
                db.Context.BookingReservations.Remove(booking);
                await db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddBookingDetailAsync(BookingDetail detail)
        {
            try
            {
                db.Context.BookingDetails.Add(detail);
                await db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateBookingDetailAsync(BookingDetail detail)
        {
            try
            {
                var existing = await db.Context.BookingDetails.FindAsync(detail.BookingReservationID, detail.RoomID);
                if (existing == null) return false;
                
                existing.StartDate = detail.StartDate;
                existing.EndDate = detail.EndDate;
                existing.ActualPrice = detail.ActualPrice;
                
                await db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteBookingDetailAsync(int bookingReservationId, int roomId)
        {
            try
            {
                var detail = await db.Context.BookingDetails.FindAsync(bookingReservationId, roomId);
                if (detail == null) return false;
                
                db.Context.BookingDetails.Remove(detail);
                await db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<BookingDetail> GetBookingDetails(int bookingReservationId)
        {
            return db.Context.BookingDetails
                .Include(d => d.RoomInformation)
                    .ThenInclude(r => r.RoomType)
                .Where(bd => bd.BookingReservationID == bookingReservationId);
        }

        public IEnumerable<BookingReservation> GetBookingsByCustomer(int customerId)
        {
            return db.Context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.RoomInformation)
                        .ThenInclude(r => r.RoomType)
                .Where(b => b.CustomerID == customerId);
        }

        public IEnumerable<BookingReservation> GetBookingsByDateRange(DateTime startDate, DateTime endDate)
        {
            return db.Context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.RoomInformation)
                        .ThenInclude(r => r.RoomType)
                .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate);
        }
    }
}