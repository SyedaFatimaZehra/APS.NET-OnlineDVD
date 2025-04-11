using DVD_World.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key]   
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [MaxLength(200)]
    public string Name { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    public string Category { get; set; } 


    [Required(ErrorMessage = "Price is required.")]
    [Range(0, 9999.99, ErrorMessage = "Price must be between 0 and 9999.99.")]
    public decimal Price { get; set; }

    public int? AlbumId { get; set; }
    public Album? Album { get; set; }

    public int? MovieId { get; set; }
    public Movie? Movie { get; set; }

    public int? GameId { get; set; }
    public Game? Game { get; set; }

    public string Description { get; set; }

    public string? ImageUrl { get; set; }

    
}
