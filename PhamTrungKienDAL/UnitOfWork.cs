using PhamTrungKienModels;

namespace PhamTrungKienDAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelDbContext _context;
        private ICustomerRepository? _customers;
        private IRoomRepository? _rooms;
        private IRoomTypeRepository? _roomTypes;
        private IRepository<BookingReservation>? _bookings;
        private IRepository<BookingDetail>? _bookingDetails;

        public UnitOfWork(HotelDbContext context)
        {
            _context = context;
        }

        public ICustomerRepository Customers => 
            _customers ??= new CustomerRepository(_context);

        public IRoomRepository Rooms => 
            _rooms ??= new RoomRepository(_context);

        public IRoomTypeRepository RoomTypes => 
            _roomTypes ??= new RoomTypeRepository(_context);

        public IRepository<BookingReservation> Bookings => 
            _bookings ??= new Repository<BookingReservation>(_context);

        public IRepository<BookingDetail> BookingDetails => 
            _bookingDetails ??= new Repository<BookingDetail>(_context);

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
            _context.Dispose();
        }
    }
}

