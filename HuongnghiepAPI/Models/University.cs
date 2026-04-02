using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Models
{
    public class University
    {
        [Key]
        public int UniversityId { get; set; }

        [Required]
        [MaxLength(300)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        // Navigation
        public ICollection<UniversityMajor> UniversityMajors { get; set; } = new List<UniversityMajor>();
    }
}
