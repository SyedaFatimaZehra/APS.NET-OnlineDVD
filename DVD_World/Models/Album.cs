using DVD_World.Models;
using System.ComponentModel.DataAnnotations;
namespace DVD_World.Models;

public class Album
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Album title is required.")]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required(ErrorMessage = "Artist is required.")]
    public int ArtistId { get; set; }

    public Artist? Artist { get; set; }

    public string? ImageUrl { get; set; }
    [Required(ErrorMessage = "Album price is required.")]
    [Range(0, 9999.99, ErrorMessage = "Price must be between 0 and 9999.99.")]
    public decimal Price { get; set; }

    public List<Song> Songs { get; set; } = new List<Song>();
}
