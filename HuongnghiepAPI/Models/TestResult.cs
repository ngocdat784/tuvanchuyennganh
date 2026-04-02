using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Models
{
    public class TestResult
    {
        [Key]
        public int TestResultId { get; set; }

        // Thời điểm nộp bài test (UTC)
        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Tổng thời gian làm bài (tính bằng giây)
        public int? DurationSeconds { get; set; }

        // Nếu bài test của học sinh đã đăng ký
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        // Danh sách các câu trả lời của bài test
        public ICollection<TestAnswer> TestAnswers { get; set; } = new List<TestAnswer>();

        // Danh sách điểm ngành (kết quả cuối cùng)
        public ICollection<TestResultMajorScore> MajorScores { get; set; } = new List<TestResultMajorScore>();
    }
}
