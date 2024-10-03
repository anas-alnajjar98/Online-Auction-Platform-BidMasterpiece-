using System.Security.Cryptography;

namespace MasterPiece.Models
{
    public class Auction
    {
        public int AuctionId { get; set; }            // Primary Key
        public DateTime StartTime { get; set; }       // Start time of the auction
        public DateTime EndTime { get; set; }         // End time of the auction
        public decimal CurrentHighestBid { get; set; }// Current highest bid
        public int? CurrentHighestBidderId { get; set; }  // Foreign Key for User

        // Foreign Key for Product
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string AuctionStatus { get; set; } = "ongoing";

        public bool IsNotificationSent { get; set; } = false;
        public User CurrentHighestBidder { get; set; }  // Navigation property for highest bidder

        // Navigation Properties
        public ICollection<Bid> Bids { get; set; }
    }

}
