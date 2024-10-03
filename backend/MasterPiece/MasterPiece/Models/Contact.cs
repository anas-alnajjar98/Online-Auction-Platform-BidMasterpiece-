namespace MasterPiece.Models
{
    public class Contact
    {
        public int ContactId { get; set; }            // Primary Key
        public string Name { get; set; }              // Name of the person contacting
        public string Email { get; set; }             // Email of the person contacting
        public string Subject { get; set; }           // Subject of the message
        public string Message { get; set; }           // Content of the message
        public DateTime SubmittedAt { get; set; } = DateTime.Now;  // Date of submission

        // Optional Foreign Key for User
        public int? UserId { get; set; }
        public User User { get; set; }
    }

}
