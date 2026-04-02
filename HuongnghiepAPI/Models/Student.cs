using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

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

        // ✅ ROLE (student / admin)
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "student";

        [MaxLength(300)]
        public string? School { get; set; }

        public int? Grade { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        public ICollection<TestResult>? TestResults { get; set; }
    }
}
