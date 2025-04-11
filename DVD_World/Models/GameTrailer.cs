using DVD_World.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace DVD_World.Models;

public class GameTrailer
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Trailer title is required.")]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required(ErrorMessage = "Game selection is required.")]
    public int GameId { get; set; }
    public Game? Game { get; set; }

    public string? TrailerUrl { get; set; }

    public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;

}