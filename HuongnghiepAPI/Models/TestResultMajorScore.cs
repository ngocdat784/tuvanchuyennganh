using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Models
{
    public class TestResultMajorScore
    {
        [Key]
        public int ScoreId { get; set; }

        // Điểm số cuối cùng của ngành sau khi tính toán
        [Required]
        public int Score { get; set; }

        // Thuộc bài test nào
        [Required]
        public int TestResultId { get; set; }
        public TestResult TestResult { get; set; } = default!;

        // Thuộc ngành nào
        [Required]
        public int MajorId { get; set; }
        public Major Major { get; set; } = default!;
    }
}
