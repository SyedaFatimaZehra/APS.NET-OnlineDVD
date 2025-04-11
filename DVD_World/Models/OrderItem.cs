namespace DVD_World.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }  // Foreign Key
        public Order Order { get; set; }  // Navigation Property

        public int ProductId { get; set; }
        public Product Product { get; set; } // Navigation Property

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
