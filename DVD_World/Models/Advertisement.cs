using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models
{
    public class Advertisement
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Banner URL is required.")]
     
        public string? Name { get; set; }
        public string? BannerUrl { get; set; }
        //[Required(ErrorMessage = "Ad position is required.")]
        //public string? Position { get; set; }
    }
}
