using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CareerOrientationAPI.Models
{
    public class AnswerImpact
    {
        [Key]
        public int ImpactId { get; set; }

        // Mức độ ảnh hưởng từ -5 đến +5, hoặc tùy bạn thiết kế
        [Required]
        public int ImpactValue { get; set; }

        // --------------------------
        //  FOREIGN KEYS & NAVIGATION
        // --------------------------

        // Liên kết Answer
        [Required]
        public int AnswerId { get; set; }
        [JsonIgnore]
        public Answer Answer { get; set; } = default!;

        // Liên kết Major
        [Required]
        public int MajorId { get; set; }
        [JsonIgnore]
        public Major Major { get; set; } = default!;

        // Liên kết đến Trait của ngành (ví dụ: Logic, Sáng tạo…)
        [Required]
        public int MajorTraitId { get; set; }
        [JsonIgnore]
        public MajorTrait MajorTrait { get; set; } = default!;
    }
}
