using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(20)]
        public string Password { get; set; }

        public string? ImageUrl { get; set; }

    }
}
