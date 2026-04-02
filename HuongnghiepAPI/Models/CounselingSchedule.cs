using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerOrientationAPI.Models
{
    public class CounselingSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        // FK tới Counselor
        [Required]
        public int CounselorId { get; set; }

        [ForeignKey(nameof(CounselorId))]
        public Counselor? Counselor { get; set; }

        // ID học sinh hoặc bạn có thể tạo bảng Student
        [Required]
        public int StudentId { get; set; }  // tạm thời để int
        // Nếu bạn chưa có bảng Student, có thể đổi thành:
        // public string StudentName { get; set; }

        // Thời gian đặt lịch
        [Required]
        public DateTime BookingTime { get; set; }

        // pending, approved, done, cancelled
        [MaxLength(50)]
        public string Status { get; set; } = "pending";

        // Ghi chú thêm
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
