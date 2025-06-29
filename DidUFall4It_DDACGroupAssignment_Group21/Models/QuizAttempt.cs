namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuizID { get; set; }
        public int Score { get; set; }
        public string? Notes { get; set; }
        public DateTime AttemptDate { get; set; }
        public int InformativeRating { get; set; } 
        public int EngagementRating { get; set; } 
    }
}
