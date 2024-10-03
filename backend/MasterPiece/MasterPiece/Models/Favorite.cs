namespace MasterPiece.Models
{
    public class Favorite
    {
        public int FavoriteId { get; set; }           // Primary Key

        // Foreign Key for User
        public int UserId { get; set; }
        public User User { get; set; }

        // Foreign Key for Product (if it's a product favorite)
        public int? ProductId { get; set; }
        public Product Product { get; set; }

        // Foreign Key for Blog (if it's a blog favorite)
        public int? BlogId { get; set; }
        public Blog Blog { get; set; }
    }

}
