using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaypic_Web3.Models
{
    public class ConversationGroupeVM
    {
        public int ConversationId { get; set; }
        public int CurrentUserId { get; set; }
        public string Titre { get; set; }
        public DateTime? HeaderDate { get; set; }
        public List<int> UtilisateurIds { get; set; } = new();
        public List<HistoriqueMessages> Messages { get; set; } = new();
    }
}