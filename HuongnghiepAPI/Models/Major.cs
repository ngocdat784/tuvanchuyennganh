using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CareerOrientationAPI.Models
{
    public class Major
    {
        [Key]
        public int MajorId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? Details { get; set; }

        // ===============================
        // 🔥 THÔNG TIN CHI TIẾT NGÀNH
        // ===============================
        public string? Difficulty { get; set; }
        public string? Career { get; set; }

        // ===============================
        // 🧠 KEYWORDS ĐỘNG (MỚI – CHO CHATBOT)
        // ===============================
        public string? Keywords { get; set; }   // 👈 CỘT MỚI (NULL OK)

        // ===============================
        // Navigation Properties
        // ===============================
        public ICollection<MajorTrait> MajorTraits { get; set; } = new List<MajorTrait>();
        public ICollection<AnswerImpact> AnswerImpacts { get; set; } = new List<AnswerImpact>();

        [JsonIgnore]
        public ICollection<TestResultMajorScore> TestResultScores { get; set; } = new List<TestResultMajorScore>();

        public ICollection<UniversityMajor> UniversityMajors { get; set; } = new List<UniversityMajor>();
    }
}
