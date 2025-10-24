using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhamTrungKienModels
{
    [Table("BookingReservation")]
    public class BookingReservation
    {
        [Key]
        public int BookingReservationID { get; set; }
        
        public DateTime? BookingDate { get; set; }
        
        [Column(TypeName = "money")]
        public decimal? TotalPrice { get; set; }
        
        [Required]
        public int CustomerID { get; set; }
        
        public byte? BookingStatus { get; set; }
        
        // Navigation properties
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; } = null!;
        
        public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    }

    [Table("BookingDetail")]
    public class BookingDetail
    {
        [Key]
        [Column(Order = 0)]
        public int BookingReservationID { get; set; }
        
        [Key]
        [Column(Order = 1)]
        public int RoomID { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Column(TypeName = "money")]
        public decimal? ActualPrice { get; set; }
        
        // Navigation properties
        [ForeignKey("BookingReservationID")]
        public virtual BookingReservation BookingReservation { get; set; } = null!;
        
        [ForeignKey("RoomID")]
        public virtual RoomInformation RoomInformation { get; set; } = null!;
    }
}

