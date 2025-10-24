using PhamTrungKienModels;

namespace PhamTrungKienDAL
{
    public interface IRoomRepository : IRepository<RoomInformation>
    {
        IEnumerable<RoomInformation> GetActiveRooms();
        IEnumerable<RoomInformation> GetRoomsByType(int roomTypeId);
        Task<IEnumerable<RoomInformation>> GetActiveRoomsAsync();
    }
}

