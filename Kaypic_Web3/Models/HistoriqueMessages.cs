using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaypic_Web3.Models
{
    public class HistoriqueMessages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Message { get; set; }
        public string NomEnvoyeur { get; set; }
        public int IdEnvoyeur { get; set; }
        public DateTime DateEnvoi { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        
        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    }
} 