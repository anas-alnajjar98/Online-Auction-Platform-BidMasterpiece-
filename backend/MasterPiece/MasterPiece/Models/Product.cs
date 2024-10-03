namespace MasterPiece.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Use as publish date
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Foreign Key for the seller (User)
        public int SellerId { get; set; }
        public User Seller { get; set; }

        // Additional fields for the product
        public int UnitsSold { get; set; } = 0;         // Track how many units of this product are sold
        public int Stock { get; set; }                  // Track available stock
        public string Condition { get; set; }           // New, Used, Refurbished, etc.
        public string Location { get; set; }            // Location of the product
        public string Country { get; set; }             // Country of origin or sale
        public string Brand { get; set; }               // Product brand
        public int Views { get; set; } = 0;             // Track number of views
        public int Quantity { get; set; } = 1;          // Quantity available

        // Foreign Key for Category
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string ApprovalStatus { get; set; } = "Pending";  // Default approval status

        // Navigation properties
        public ICollection<Auction> Auctions { get; set; }
    }
}
