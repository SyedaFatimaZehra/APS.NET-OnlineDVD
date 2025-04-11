using DVD_World.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class PurchasingInvoice
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Supplier Id is required.")]
    public int SupplierId { get; set; }

    public Supplier Supplier { get; set; }

    [Required(ErrorMessage = "Product Id is required.")]

    public int? ProductId { get; set; }

    public Product Product { get; set; }

    [Required(ErrorMessage = "Invoice details are required.")]
    public string InvoiceDetails { get; set; }


    [Required(ErrorMessage = "Purchase date is required.")]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Total cost is required.")]
    [Range(0, 999999.99, ErrorMessage = "Total cost must be between 0 and 999999.99.")]
    public decimal TotalCost { get; set; }
}