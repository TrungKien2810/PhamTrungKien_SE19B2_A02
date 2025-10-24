using System.Collections.Generic;
using System.Linq;
using PhamTrungKienDAL;
using PhamTrungKienModels;
using System.Threading.Tasks;

namespace PhamTrungKienBLL
{
    public class RoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomService()
        {
            _roomRepository = DatabaseService.Instance.UnitOfWork.Rooms;
            _roomTypeRepository = DatabaseService.Instance.UnitOfWork.RoomTypes;
        }

        public IEnumerable<RoomInformation> GetAll() => _roomRepository.GetActiveRooms();

        public RoomInformation? GetById(int id) => _roomRepository.GetById(id);

        public IEnumerable<RoomType> GetAllRoomTypes() => _roomTypeRepository.GetAll();

        public async Task<bool> AddAsync(RoomInformation room)
        {
            try
            {
                if (!ValidateRoom(room))
                    return false;

                // Check if room number already exists
                if (GetByRoomNumber(room.RoomNumber) != null)
                    return false;

                room.RoomStatus = 1;
                _roomRepository.Add(room);
                await _roomRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(RoomInformation room)
        {
            try
            {
                if (!ValidateRoom(room))
                {
                    System.Diagnostics.Debug.WriteLine($"Room validation failed: RoomNumber={room.RoomNumber}, RoomTypeID={room.RoomTypeID}");
                    return false;
                }

                var existingRoom = _roomRepository.GetById(room.RoomID);
                if (existingRoom == null) 
                {
                    System.Diagnostics.Debug.WriteLine($"Room not found: ID={room.RoomID}");
                    return false;
                }

                // Check if room number already exists for another room
                var roomWithSameNumber = GetByRoomNumber(room.RoomNumber);
                if (roomWithSameNumber != null && roomWithSameNumber.RoomID != room.RoomID)
                {
                    System.Diagnostics.Debug.WriteLine($"Room number already exists: {room.RoomNumber}");
                    return false;
                }

                // Update properties
                existingRoom.RoomNumber = room.RoomNumber;
                existingRoom.RoomDetailDescription = room.RoomDetailDescription;
                existingRoom.RoomMaxCapacity = room.RoomMaxCapacity;
                existingRoom.RoomPricePerDay = room.RoomPricePerDay;
                existingRoom.RoomTypeID = room.RoomTypeID;

                // Mark as modified and save
                _roomRepository.Update(existingRoom);
                await _roomRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating room: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var existingRoom = _roomRepository.GetById(id);
                if (existingRoom == null) return false;

                // Check if room has any bookings
                var bookingService = new BookingService();
                var roomBookings = bookingService.GetAll()
                    .SelectMany(b => b.BookingDetails)
                    .Where(bd => bd.RoomID == id);
                
                if (roomBookings.Any())
                {
                    // Soft delete - set status to deleted
                    existingRoom.RoomStatus = 2;
                    _roomRepository.Update(existingRoom);
                }
                else
                {
                    // Hard delete if no bookings
                    _roomRepository.Remove(existingRoom);
                }
                
                await _roomRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<RoomInformation> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            return _roomRepository.Find(r => 
                (r.RoomNumber != null && r.RoomNumber.Contains(searchTerm)) ||
                (r.RoomDetailDescription != null && r.RoomDetailDescription.Contains(searchTerm))
            );
        }

        public IEnumerable<RoomInformation> GetRoomsByType(int roomTypeId)
        {
            return _roomRepository.GetRoomsByType(roomTypeId);
        }

        public IEnumerable<RoomInformation> GetAvailableRooms(DateTime startDate, DateTime endDate)
        {
            var bookingService = new BookingService();
            var bookedRoomIds = bookingService.GetAll()
                .SelectMany(b => b.BookingDetails)
                .Where(bd => !(bd.EndDate <= startDate || bd.StartDate >= endDate))
                .Select(bd => bd.RoomID)
                .Distinct();

            return GetAll().Where(r => !bookedRoomIds.Contains(r.RoomID));
        }

        public RoomInformation? GetByRoomNumber(string roomNumber)
        {
            return _roomRepository.Find(r => r.RoomNumber == roomNumber).FirstOrDefault();
        }

        public bool TestConnection()
        {
            try
            {
                var testRoom = _roomRepository.GetAll().FirstOrDefault();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection test failed: {ex.Message}");
                return false;
            }
        }

        private bool ValidateRoom(RoomInformation room)
        {
            if (string.IsNullOrWhiteSpace(room.RoomNumber))
                return false;

            if (room.RoomTypeID <= 0)
                return false;

            if (room.RoomMaxCapacity.HasValue && room.RoomMaxCapacity.Value <= 0)
                return false;

            if (room.RoomPricePerDay.HasValue && room.RoomPricePerDay.Value < 0)
                return false;

            return true;
        }
    }
}