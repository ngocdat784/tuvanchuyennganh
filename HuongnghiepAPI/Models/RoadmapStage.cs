using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CareerOrientationAPI.Models
{
    public class RoadmapStage
    {
        [Key]
        public int StageId { get; set; }

        [Required]
        public int RoadmapId { get; set; }

        [Required]
        public int StageOrder { get; set; }

        [Required]
        [MaxLength(255)]
        public string StageTitle { get; set; } = string.Empty;

        public string? StageDescription { get; set; }

        // --- Navigation ---
        public Roadmap? Roadmap { get; set; }

        public ICollection<RoadmapItem> Items { get; set; } = new List<RoadmapItem>();
    }
}
