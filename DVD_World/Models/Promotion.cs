using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Promotion
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Promotion description is required.")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Discount percentage is required."), Range(0, 100, ErrorMessage = "Discount must be between 0 and 100%.")]
        public decimal DiscountPercentage { get; set; }
    }
}
