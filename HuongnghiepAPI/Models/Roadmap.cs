using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CareerOrientationAPI.Models
{
    public class Roadmap
    {
        [Key]
        public int RoadmapId { get; set; }

        [Required]
        public int MajorId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        // --- Navigation ---
        public Major? Major { get; set; }

        public ICollection<RoadmapStage> Stages { get; set; } = new List<RoadmapStage>();
    }
}
