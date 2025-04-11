using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Contact
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Message { get; set; }



    }
}
