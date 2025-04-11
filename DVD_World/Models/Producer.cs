using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Producer
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Producer name is required."), MaxLength(100)]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Contact information is required.")]
        public string? ContactInfo { get; set; }
    }
}
