namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class LearningGoal
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Goal { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }  
        public DateTime EndDate { get; set; }    
        public int DurationDays { get; set; }    
    }

}
