using System.ComponentModel.DataAnnotations.Schema;

namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class QuizModel
    {
        public int QuizModelId { get; set; }
        public string? Title { get; set; }
        public List<Question>? QuestionIds { get; set; } = new();
        public List<QuizReview>? QuizReviews { get; set; }
        [NotMapped]
        public bool hasAttempted { get; set; }
    }
}
