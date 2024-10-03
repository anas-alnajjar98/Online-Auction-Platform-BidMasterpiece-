namespace MasterPiece.Dtos
{
    public class EditUserResetPasswordDto
    {
       public int UserID { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
