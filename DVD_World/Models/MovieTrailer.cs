using DVD_World.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace DVD_World.Models;

public class MovieTrailer
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Trailer title is required.")]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required(ErrorMessage = "Movie Selection is required.")]
    public int MovieId { get; set; }

    public Movie? Movie { get; set; }

    //[Required(ErrorMessage = "Trailer URL is required.")]
    public string? TrailerUrl { get; set; }

    public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;

}
