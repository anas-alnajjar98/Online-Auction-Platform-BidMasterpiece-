namespace MasterPiece.Models
{

    public class Blog
    {

        //Images
        public int BlogId { get; set; }               // Primary Key
        public string Title { get; set; }             // Blog title
        public string Content { get; set; }           // Blog content
        public DateTime PublishedAt { get; set; } = DateTime.Now; // Date of submission

        // Foreign Key for User (Author of the blog)
        public int UserId { get; set; }               // Foreign Key to User
        public User Author { get; set; }              // Navigation property for the author
        public int ViewCount { get; set; } = 0;
        public string ImageUrl { get; set; }  // URL or path for the blog image

        // Admin approval status for blog
        public string ApprovalStatus { get; set; } = "Pending"; // Default to "Pending"
    }

}
