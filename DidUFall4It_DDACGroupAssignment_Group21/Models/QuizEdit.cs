namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class QuizEdit
    {
        public List<QuizModel>? Quizzes { get; set; }
        public QuizModel? SelectedQuiz { get; set; }
        public List<Question>? AllQuestions { get; set; }
    }
}
