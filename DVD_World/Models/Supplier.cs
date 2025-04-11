using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Supplier name is required."), MaxLength(100)]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Contact information is required.")]
        public string? ContactInfo { get; set; }
    }
}
