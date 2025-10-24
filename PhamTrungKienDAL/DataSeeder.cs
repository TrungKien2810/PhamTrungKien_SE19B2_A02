using Microsoft.EntityFrameworkCore;
using PhamTrungKienModels;

namespace PhamTrungKienDAL
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(HotelDbContext context)
        {
            // Check if data already exists
            if (await context.RoomTypes.AnyAsync())
                return;

            // Seed RoomTypes
            var roomTypes = new List<RoomType>
            {
                new RoomType { RoomTypeName = "Standard", TypeDescription = "Standard room with basic amenities", TypeNote = "Basic room type" },
                new RoomType { RoomTypeName = "Deluxe", TypeDescription = "Deluxe room with premium amenities", TypeNote = "Premium room type" },
                new RoomType { RoomTypeName = "Suite", TypeDescription = "Luxury suite with full amenities", TypeNote = "Luxury room type" },
                new RoomType { RoomTypeName = "Family", TypeDescription = "Family room for multiple guests", TypeNote = "Family-friendly room type" },
                new RoomType { RoomTypeName = "Executive", TypeDescription = "Executive room for business travelers", TypeNote = "Business room type" }
            };

            await context.RoomTypes.AddRangeAsync(roomTypes);
            await context.SaveChangesAsync();

            // Update existing rooms with RoomTypeID if they don't have one
            var rooms = await context.RoomInformations.Where(r => r.RoomTypeID == 0).ToListAsync();
            if (rooms.Any())
            {
                var standardRoomType = await context.RoomTypes.FirstOrDefaultAsync(rt => rt.RoomTypeName == "Standard");
                if (standardRoomType != null)
                {
                    foreach (var room in rooms)
                    {
                        room.RoomTypeID = standardRoomType.RoomTypeID;
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}

