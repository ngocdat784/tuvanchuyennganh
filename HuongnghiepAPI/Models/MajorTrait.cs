using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CareerOrientationAPI.Models
{
    public class MajorTrait
    {
        [Key]
        public int TraitId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        // Foreign Key → Major
        [Required]
        public int MajorId { get; set; }

        // Navigation (một Major có nhiều Trait)
        [JsonIgnore]
        public Major Major { get; set; } = default!;
    }
}
