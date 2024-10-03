namespace MasterPiece.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }     // Primary Key
        public string Message { get; set; }         // Notification message content
        public bool IsRead { get; set; } = false;   // Has the user read the notification
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Date notification was created

        // Foreign Key for User
        public int UserId { get; set; }
        public User User { get; set; }

        // Optional: Foreign Key for Product, Auction, or Blog (to relate the notification)
        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public int? AuctionId { get; set; }
        public Auction Auction { get; set; }

        public int? BlogId { get; set; }
        public Blog Blog { get; set; }
    }

}
