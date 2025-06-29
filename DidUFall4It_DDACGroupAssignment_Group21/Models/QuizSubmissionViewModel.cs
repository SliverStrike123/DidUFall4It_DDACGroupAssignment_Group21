namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class QuizSubmissionViewModel
    {
        public int QuizID { get; set; }

        public Dictionary<int, int> SubmittedAnswers { get; set; } = new(); // QuestionId -> SelectedOptionNumber

        public string Notes { get; set; } = string.Empty;

        public int InformativeRating { get; set; }  // 1 to 5
        public int EngagementRating { get; set; }   // 1 to 5
    }
}
