using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Models
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;      // Nội dung đáp án

        public int Order { get; set; } = 0;                   // Thứ tự trong câu hỏi

        // Foreign Key → Question
        [Required]
        public int QuestionId { get; set; }

        // Navigation: một Answer thuộc 1 Question
        public Question Question { get; set; } = default!;

        // Navigation: 1 Answer có thể tác động đến nhiều ngành / trait
        public ICollection<AnswerImpact> Impacts { get; set; } = new List<AnswerImpact>();
    }
}
