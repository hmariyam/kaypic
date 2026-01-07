namespace Kaypic_Web3.Models
{
    public class HomeVM
    {
        public List<Conversation> Conversations { get; set; } = new();
        public ConversationVM? Current { get; set; }
        public List<Annonce>? Annonces { get; set; }
        public IEnumerable<Calendrier>? Calendriers { get; set; }
    }
}