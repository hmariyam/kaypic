using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaypic_Web3.Models
{
    public class HistoriqueAppels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Id_appelleur { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public DateTime startedAt { get; set; }
        public DateTime? endedAt { get; set; }
    }
}
