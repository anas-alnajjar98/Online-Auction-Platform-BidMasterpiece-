namespace MasterPiece.Models
{
    public class OrderHistory
    {
        public int OrderHistoryId { get; set; }       // Primary Key
        public int AuctionId { get; set; }            // Foreign Key for Auction
        public Auction Auction { get; set; }

        // Foreign Key for the User who won the auction
        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;  // Date when the order was placed
        public decimal TotalAmount { get; set; }                // Total amount for the order
    }
}
