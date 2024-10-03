namespace MasterPiece.Models
{
    public class UserRole
    {
        public int UserRoleId { get; set; }
        public string RoleName { get; set; }

        // Foreign Key for User
        public int UserId { get; set; }
        public User User { get; set; }
    }

}
