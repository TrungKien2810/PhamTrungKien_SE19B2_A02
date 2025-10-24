using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhamTrungKienModels
{
    [Table("RoomType")]
    public class RoomType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomTypeID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RoomTypeName { get; set; } = string.Empty;
        
        [StringLength(250)]
        public string? TypeDescription { get; set; }
        
        [StringLength(250)]
        public string? TypeNote { get; set; }
        
        // Navigation properties
        public virtual ICollection<RoomInformation> RoomInformations { get; set; } = new List<RoomInformation>();
    }
}

