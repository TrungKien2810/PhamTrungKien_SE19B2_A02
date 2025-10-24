using Microsoft.EntityFrameworkCore;
using PhamTrungKienModels;

namespace PhamTrungKienDAL
{
    public class DatabaseService
    {
        private static DatabaseService? _instance;
        private static readonly object _lock = new object();
        private readonly HotelDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        private DatabaseService()
        {
            var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();
            DatabaseConfig.ConfigureDbContext(optionsBuilder);
            _context = new HotelDbContext(optionsBuilder.Options);
            _unitOfWork = new UnitOfWork(_context);
            
            // Seed data if needed
            _ = Task.Run(async () => await DataSeeder.SeedDataAsync(_context));
        }

        public static DatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DatabaseService();
                        }
                    }
                }
                return _instance;
            }
        }

        public HotelDbContext Context => _context;
        public IUnitOfWork UnitOfWork => _unitOfWork;

        // Legacy properties for backward compatibility
        public IQueryable<Customer> Customers => _context.Customers;
        public IQueryable<RoomType> RoomTypes => _context.RoomTypes;
        public IQueryable<RoomInformation> RoomInformations => _context.RoomInformations;
        public IQueryable<BookingReservation> BookingReservations => _context.BookingReservations;
        public IQueryable<BookingDetail> BookingDetails => _context.BookingDetails;

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
            _context.Dispose();
        }
    }
}

