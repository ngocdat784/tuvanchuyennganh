using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CareerOrientationAPI.Models
{
    public class RoadmapSkill
    {
        [Key]
        public int SkillId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        [MaxLength(255)]
        public string SkillName { get; set; } = string.Empty;

        public string? Description { get; set; }

        // --- Navigation ---
        [JsonIgnore]
        public RoadmapItem? Item { get; set; }
    }
}
