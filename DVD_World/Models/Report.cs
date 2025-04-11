using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Report title is required."), MaxLength(200)]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Report content is required.")]
        public string? Content { get; set; }
    }
}
