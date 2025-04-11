using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required(ErrorMessage = "Feedback message is required.")]
        public string? Message { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } 
    }
}
