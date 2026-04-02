using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;   // Nội dung câu hỏi

        public int Order { get; set; } = 0;                // Thứ tự trong nhóm

        // Foreign Key → QuestionGroup
        [Required]
        public int QuestionGroupId { get; set; }

        // Navigation: Mỗi câu hỏi thuộc 1 nhóm
        public QuestionGroup? QuestionGroup { get; set; }

        // Navigation: Một câu hỏi có nhiều câu trả lời
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
