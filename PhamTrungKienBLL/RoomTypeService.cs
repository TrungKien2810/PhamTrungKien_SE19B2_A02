using System.Collections.Generic;
using System.Linq;
using PhamTrungKienDAL;
using PhamTrungKienModels;
using System.Threading.Tasks;

namespace PhamTrungKienBLL
{
    public class RoomTypeService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomTypeService()
        {
            _roomTypeRepository = DatabaseService.Instance.UnitOfWork.RoomTypes;
        }

        public IEnumerable<RoomType> GetAll() => _roomTypeRepository.GetAll();

        public RoomType? GetById(int id) => _roomTypeRepository.GetById(id);

        public async Task<bool> AddAsync(RoomType roomType)
        {
            try
            {
                _roomTypeRepository.Add(roomType);
                await _roomTypeRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(RoomType roomType)
        {
            try
            {
                var existingRoomType = _roomTypeRepository.GetById(roomType.RoomTypeID);
                if (existingRoomType == null) 
                {
                    System.Diagnostics.Debug.WriteLine($"RoomType not found: ID={roomType.RoomTypeID}");
                    return false;
                }

                existingRoomType.RoomTypeName = roomType.RoomTypeName;
                existingRoomType.TypeDescription = roomType.TypeDescription;
                existingRoomType.TypeNote = roomType.TypeNote;

                _roomTypeRepository.Update(existingRoomType);
                await _roomTypeRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating room type: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var existingRoomType = _roomTypeRepository.GetById(id);
                if (existingRoomType == null) return false;

                // Check if room type is being used by any rooms
                var roomService = new RoomService();
                var roomsUsingType = roomService.GetRoomsByType(id);
                if (roomsUsingType.Any())
                {
                    return false; // Cannot delete room type that is being used
                }

                _roomTypeRepository.Remove(existingRoomType);
                await _roomTypeRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<RoomType> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            return _roomTypeRepository.Find(rt => 
                (rt.RoomTypeName != null && rt.RoomTypeName.Contains(searchTerm)) ||
                (rt.TypeDescription != null && rt.TypeDescription.Contains(searchTerm)) ||
                (rt.TypeNote != null && rt.TypeNote.Contains(searchTerm))
            );
        }
    }
}
