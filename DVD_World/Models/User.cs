using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Fullname is required."), MaxLength(50)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Username is required."), MaxLength(50)]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Email is required."), EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required."), MinLength(8), MaxLength(20)]
        public string? Password { get; set; }

        public string? Image { get; set; }

        public string Role { get; set; } = "u";
    }
}