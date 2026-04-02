using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerOrientationAPI.Models
{
    public class MajorSubField
    {
        [Key]
        public int SubFieldId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        // Foreign Key → Major
        [Required]
        public int MajorId { get; set; }

        // Navigation (một Major có nhiều SubFields)
        public Major Major { get; set; } = default!;
    }
}
