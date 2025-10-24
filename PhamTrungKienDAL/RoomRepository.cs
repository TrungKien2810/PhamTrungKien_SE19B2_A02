using Microsoft.EntityFrameworkCore;
using PhamTrungKienModels;

namespace PhamTrungKienDAL
{
    public class RoomRepository : Repository<RoomInformation>, IRoomRepository
    {
        public RoomRepository(HotelDbContext context) : base(context)
        {
        }

        public IEnumerable<RoomInformation> GetActiveRooms()
        {
            return _dbSet
                .Include(r => r.RoomType)
                .Where(r => r.RoomStatus == 1)
                .ToList();
        }

        public IEnumerable<RoomInformation> GetRoomsByType(int roomTypeId)
        {
            return _dbSet
                .Include(r => r.RoomType)
                .Where(r => r.RoomTypeID == roomTypeId && r.RoomStatus == 1)
                .ToList();
        }

        public async Task<IEnumerable<RoomInformation>> GetActiveRoomsAsync()
        {
            return await _dbSet
                .Include(r => r.RoomType)
                .Where(r => r.RoomStatus == 1)
                .ToListAsync();
        }
    }
}

