namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class QuizHistoryViewModel
    {
        public string QuizTitle { get; set; } = "";
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; }
        public QuizModel? Quiz { get; set; }
    }
}
