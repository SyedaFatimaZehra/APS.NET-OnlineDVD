using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Artist name is required."), MaxLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Artist bio is required.")]
        public string? Bio { get; set; }

        public string? ImageUrl { get; set; }

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
