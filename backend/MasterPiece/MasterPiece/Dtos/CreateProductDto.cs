namespace MasterPiece.Dtos
{
    public class CreateProductDto
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public int Stock { get; set; }
        public string Condition { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public string Brand { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }

        // File upload property
        public IFormFile Image { get; set; }

        // Add the UserId field to accept from local storage
        public int UserId { get; set; }
    }

}
