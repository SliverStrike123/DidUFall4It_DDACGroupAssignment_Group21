using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DidUFall4It_DDACGroupAssignment_Group21.Models
{
    public class InfographicModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string? Tips { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public List<InfographicFeedback>? InfograpohicFeedbacks { get; set; }
    }

}
