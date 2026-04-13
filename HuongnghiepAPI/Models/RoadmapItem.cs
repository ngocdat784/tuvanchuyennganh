using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CareerOrientationAPI.Models
{
    public class RoadmapItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        public int StageId { get; set; }

        [Required]
        public int ItemOrder { get; set; }

        [Required]
        [MaxLength(255)]
        public string ItemTitle { get; set; } = string.Empty;

        public string? Content { get; set; }

        [MaxLength(500)]
        public string? ResourceLink { get; set; }

        // --- Navigation ---
        [JsonIgnore]
        public RoadmapStage? Stage { get; set; }

        public ICollection<RoadmapSkill> Skills { get; set; } = new List<RoadmapSkill>();
    }
}
