using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Movie Name is required.")]
        [MaxLength(200)]
        public string MovieName { get; set; }

        [Required(ErrorMessage = "Movie description is required.")]
        public string Description { get; set; }

        public string? MovieUrl { get; set; }
        public string? MovieImage { get; set; }
    }
}
