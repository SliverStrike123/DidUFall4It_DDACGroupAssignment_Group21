namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class InfographicFeedbackViewModel
    {
        public List<InfographicFeedback> FeedbackList { get; set; } = new List<InfographicFeedback>();

        public int TotalCount
        {
            get
            {
                return FeedbackList != null ? FeedbackList.Count : 0;
            }
        }
    }
}
