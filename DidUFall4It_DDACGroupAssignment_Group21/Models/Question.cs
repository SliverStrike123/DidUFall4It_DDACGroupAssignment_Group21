namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string? QuestionText { get; set; }
        List<string>? Options { get; set; }
        public int? Answer { get; set; }
        public int? Score { get; set; }
    }
}
