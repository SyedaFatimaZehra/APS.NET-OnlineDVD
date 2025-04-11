using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DVD_World.Models;
public class CheckoutViewModel
{
    public List<Product> Products { get; set; } = new List<Product>();



    [Required(ErrorMessage = "Full Name is required.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }

    public decimal TotalAmount { get; set; }

}
