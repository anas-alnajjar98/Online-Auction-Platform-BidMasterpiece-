namespace MasterPiece.Dtos
{
    public class GetAuctionDetailsDto
    {
        public int AuctionId { get; set; }                  // Auction ID
        public string ProductName { get; set; }             // Product name
        public decimal CurrentHighestBid { get; set; }      // Current highest bid
        public decimal ShippingCost { get; set; } = 20;     // Example shipping cost
        public decimal Tax { get; set; } = 5;               // Example tax
        public decimal TotalAmount => CurrentHighestBid + ShippingCost + Tax;   // Total amount calculated
    }
}
