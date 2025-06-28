namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public string? OptionOne { get; set; }
        public string? OptionTwo { get; set; }
        public string? OptionThree { get; set; }
        public string? OptionFour { get; set; }
        public int? Answer { get; set; }
        public int? Score { get; set; }
    }
}
