using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Models
{
    public class TestAnswerImpact
    {
        [Key]
        public int TestAnswerImpactId { get; set; }

        // Giá trị ảnh hưởng thực tế (-5 → +5 hoặc tùy thiết kế)
        [Required]
        public int ImpactValue { get; set; }

        // Thuộc TestAnswer nào
        [Required]
        public int TestAnswerId { get; set; }
        public TestAnswer TestAnswer { get; set; } = default!;

        // Major chịu ảnh hưởng
        [Required]
        public int MajorId { get; set; }
        public Major Major { get; set; } = default!;

        // Trait chịu ảnh hưởng
        [Required]
        public int MajorTraitId { get; set; }
        public MajorTrait MajorTrait { get; set; } = default!;
    }
}
