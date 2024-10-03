namespace MasterPiece.Models
{
    public class Category
    {
        public int CategoryId { get; set; }           // Primary Key
        public string CategoryName { get; set; }      // Name of the category

        // Navigation Properties
        public ICollection<Product> Products { get; set; }
    }

}
