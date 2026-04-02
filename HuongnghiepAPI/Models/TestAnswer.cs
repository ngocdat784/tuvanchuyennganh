using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Models
{
    public class TestAnswer
    {
        [Key]
        public int TestAnswerId { get; set; }

        // Thuộc bài test nào
        [Required]
        public int TestResultId { get; set; }
        public TestResult TestResult { get; set; } = default!;

        // Câu hỏi nào
        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; } = default!;

        // Đáp án được chọn
        [Required]
        public int AnswerId { get; set; }
        public Answer Answer { get; set; } = default!;

        // Các impact tính ra từ đáp án này
        public ICollection<TestAnswerImpact> Impacts { get; set; } = new List<TestAnswerImpact>();
    }
}
