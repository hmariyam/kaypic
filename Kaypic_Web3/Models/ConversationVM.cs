namespace Kaypic_Web3.Models
{
    public class ConversationVM
    {
        public int ConversationId { get; set; }
        public int CurrentUserId { get; set; }
        public int CurrentUserIdReaction { get; set; }
        public string CurrentUserName { get; set; }
        public string Titre { get; set; }
        public DateTime? HeaderDate { get; set; }
        public List<HistoriqueMessages> Messages { get; set; } = new();
        public List<ConversationFichiers> Fichiers { get; set; } = new();
    }
}
