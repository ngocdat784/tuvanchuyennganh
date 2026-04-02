using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Models
{
    public class Counselor
    {
        [Key]
        public int CounselorId { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        // ✅ ROLE (counselor / admin)
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "counselor";

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? Details { get; set; }

        public ICollection<TestResult>? ViewedResults { get; set; }

        public ICollection<CounselingSchedule>? Schedules { get; set; }
    }
}
