using System;
using System.Collections.Generic;

namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class QuizReview
    {
        public int QuizReviewId { get; set; }
        public int? QuizId { get; set; }
        public string? Tag { get; set; }
        public double? AverageScore { get; set; }
        public int? HighestScore { get; set; }
        public int? LowestScore { get; set; }
        public List<int>? InformativeRatings { get; set; } = new();
        public List<int>? EngagementRatings { get; set; } = new();
        public List<string>? Comments { get; set; } = new();

    }
}