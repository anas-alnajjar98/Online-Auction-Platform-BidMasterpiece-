namespace MasterPiece.Dtos
{
    public class ContactDto
    {
        public string Name { get; set; }              // Name of the person contacting
        public string Email { get; set; }             // Email of the person contacting
        public string Subject { get; set; }           // Subject of the message
        public string Message { get; set; }           // Content of the message
        public int? UserId { get; set; }
    }
}
