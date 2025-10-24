using Microsoft.EntityFrameworkCore;
using PhamTrungKienModels;

namespace PhamTrungKienDAL
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<RoomInformation> RoomInformations { get; set; }
        public DbSet<BookingReservation> BookingReservations { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerID);
                entity.Property(e => e.CustomerID)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.HasIndex(e => e.EmailAddress)
                    .IsUnique();
            });

            // Configure RoomType entity
            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.HasKey(e => e.RoomTypeID);
                entity.Property(e => e.RoomTypeID)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.RoomTypeName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // Configure RoomInformation entity
            modelBuilder.Entity<RoomInformation>(entity =>
            {
                entity.HasKey(e => e.RoomID);
                entity.Property(e => e.RoomID)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.RoomNumber)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.RoomPricePerDay)
                    .HasColumnType("money");
                
                entity.HasOne(d => d.RoomType)
                    .WithMany(p => p.RoomInformations)
                    .HasForeignKey(d => d.RoomTypeID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure BookingReservation entity
            modelBuilder.Entity<BookingReservation>(entity =>
            {
                entity.HasKey(e => e.BookingReservationID);
                entity.Property(e => e.BookingReservationID)
                    .ValueGeneratedNever(); // Manual ID generation
                entity.Property(e => e.TotalPrice)
                    .HasColumnType("money");
                
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BookingReservations)
                    .HasForeignKey(d => d.CustomerID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure BookingDetail entity
            modelBuilder.Entity<BookingDetail>(entity =>
            {
                entity.HasKey(e => new { e.BookingReservationID, e.RoomID });
                entity.Property(e => e.ActualPrice)
                    .HasColumnType("money");
                
                entity.HasOne(d => d.BookingReservation)
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.BookingReservationID)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(d => d.RoomInformation)
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.RoomID)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

