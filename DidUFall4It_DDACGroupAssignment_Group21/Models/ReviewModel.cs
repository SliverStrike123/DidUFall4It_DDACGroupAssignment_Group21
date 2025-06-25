using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class ReviewModel
    {
        public int Id { get; set; }
        [Required]
        public string ReviewerName { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string ReviewText { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int InfographicId { get; set; }
        
        // Navigation property to the related Infographic
        public InfographicModel Infographic { get; set; }
    }
}
