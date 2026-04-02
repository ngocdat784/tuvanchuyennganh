using System.ComponentModel.DataAnnotations.Schema;

namespace CareerOrientationAPI.Models
{
    public class UniversityMajor
    {
        public int UniversityId { get; set; }
        public University University { get; set; } = null!;

        public int MajorId { get; set; }
        public Major Major { get; set; } = null!;

        // Độ phù hợp / ưu tiên gợi ý
        public int Priority { get; set; } = 1;
    }
}
