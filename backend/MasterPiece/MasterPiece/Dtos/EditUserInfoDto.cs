namespace MasterPiece.Dtos
{
    public class EditUserInfoDto
    {
        public string? Username { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public IFormFile? Image { get; set; }  
    }
}
