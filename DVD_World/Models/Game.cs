using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Game Name is required."), MaxLength(200)]
        public string? GameName { get; set; }
        [Required(ErrorMessage = "Game description is required.")]
        public string? Description { get; set; }
        public string? GameUrl { get; set; }
        public string? GameImage { get; set; }
    }
}
