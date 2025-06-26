namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Question { get; set; }
        public string SelectedAnswer { get; set; }
        public string Notes { get; set; }
        public DateTime AttemptDate { get; set; }
    }
}
