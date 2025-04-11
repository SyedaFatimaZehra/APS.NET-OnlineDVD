using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "News title is required.")]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "News content is required.")]
        public string? Content { get; set; }

        [Required(ErrorMessage = "News type is required.")]
        [MaxLength(50)]
        public string? Type { get; set; } // Music, Game, Movie

        public string? NewsImage { get; set; }
    }
}
