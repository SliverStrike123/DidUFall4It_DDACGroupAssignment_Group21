namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class QuizViewModel
    {
        public List<QuizModel> Quizzes { get; set; } = new List<QuizModel>();
        public int Count 
        {
            get
            {
                return Quizzes != null ? Quizzes.Count : 0;
            }
        }
    }
}
