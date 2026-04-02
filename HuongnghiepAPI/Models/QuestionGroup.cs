using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Models
{
    public class QuestionGroup
    {
        [Key]
        public int QuestionGroupId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;      // Tên nhóm

        [MaxLength(1000)]
        public string? Description { get; set; }               // Mô tả

        public int Order { get; set; } = 0;                    // Thứ tự trong bài test

        // Navigation: Một nhóm có nhiều câu hỏi
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
