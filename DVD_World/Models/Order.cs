using DVD_World.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models;
public class Order
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "User Id is required.")]
    public int UserId { get; set; }

    public User? User { get; set; }


    [Required(ErrorMessage = "Product Id is required.")]
    public int ProductId { get; set; }
    public Product Product { get; set; }

    [Required(ErrorMessage = "Order details are required.")]

    public string OrderDetails { get; set; }

    [Required(ErrorMessage = "Order date is required.")]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;


    [Required(ErrorMessage = "Total amount is required.")]
    [Range(0, 99999.99, ErrorMessage = "Total amount must be between 0 and 99999.99.")]
    public decimal TotalAmount { get; set; }


    [Required(ErrorMessage = "Order Status is required.")]
    public string Status { get; set; }

    public List<OrderItem> OrderItems { get; set; }

}

