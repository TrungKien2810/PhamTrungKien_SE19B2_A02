using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhamTrungKienModels
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }
        
        [StringLength(50)]
        public string? CustomerFullName { get; set; }
        
        [StringLength(12)]
        public string? Telephone { get; set; }
        
        [Required]
        [StringLength(50)]
        public string EmailAddress { get; set; } = string.Empty;
        
        public DateTime? CustomerBirthday { get; set; }
        
        public byte? CustomerStatus { get; set; } = 1; // 1: Active, 2: Deleted
        
        [StringLength(50)]
        public string? Password { get; set; }
        
        // Navigation properties
        public virtual ICollection<BookingReservation> BookingReservations { get; set; } = new List<BookingReservation>();
    }
}

