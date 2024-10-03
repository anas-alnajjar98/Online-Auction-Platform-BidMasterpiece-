namespace MasterPiece.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }            // Primary Key
        public decimal PaymentAmount { get; set; }    // Amount paid for the product
        public string PaymentStatus { get; set; } = "pending"; // Status of payment
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // Foreign Key for Auction
        public int AuctionId { get; set; }
        public Auction Auction { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string? PaymentLink { get; set; }

        // Foreign Key for User (Who made the payment)
        public bool IsNotificationSent { get; set; } = false;
        public int UserId { get; set; }
        public User User { get; set; }
    }

}
