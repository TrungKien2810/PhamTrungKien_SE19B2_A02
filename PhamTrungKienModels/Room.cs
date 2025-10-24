using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhamTrungKienModels
{
    [Table("RoomInformation")]
    public class RoomInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RoomNumber { get; set; } = string.Empty;
        
        [StringLength(220)]
        public string? RoomDetailDescription { get; set; }
        
        public int? RoomMaxCapacity { get; set; }
        
        [Required]
        public int RoomTypeID { get; set; }
        
        public byte? RoomStatus { get; set; } = 1; // 1: Active, 2: Deleted
        
        [Column(TypeName = "money")]
        public decimal? RoomPricePerDay { get; set; }
        
        // Navigation properties
        [ForeignKey("RoomTypeID")]
        public virtual RoomType RoomType { get; set; } = null!;
        
        public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    }
}

