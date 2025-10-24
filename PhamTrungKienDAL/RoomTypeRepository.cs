using PhamTrungKienModels;

namespace PhamTrungKienDAL
{
    public class RoomTypeRepository : Repository<RoomType>, IRoomTypeRepository
    {
        public RoomTypeRepository(HotelDbContext context) : base(context)
        {
        }
    }
}

