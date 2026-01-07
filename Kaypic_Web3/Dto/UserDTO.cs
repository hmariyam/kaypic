namespace Kaypic_Web3.Dto
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
