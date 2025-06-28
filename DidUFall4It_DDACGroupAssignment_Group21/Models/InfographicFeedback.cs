using Microsoft.EntityFrameworkCore;

namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class InfographicFeedback
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int InfographicId { get; set; }
        public int InformativeRating { get; set; }
        public int EngagementRating { get; set; }
        public int ClarityRating { get; set; }
        public int RelevanceRating { get; set; }
        public string? Comment { get; set; }
        public DateTime PostedAt { get; set; }
    }

}
