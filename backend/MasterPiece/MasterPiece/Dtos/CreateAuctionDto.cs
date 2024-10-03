namespace MasterPiece.Dtos
{
    public class CreateAuctionDto
    {
        public int ProductId { get; set; }            
        public decimal StartingPrice { get; set; }    
        public int DurationHours { get; set; }        
    }
}
