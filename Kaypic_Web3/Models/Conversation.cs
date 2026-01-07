using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaypic_Web3.Models
{
    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Status status { get; set; }
        public string Titre { get; set; }
        public DateTime date_creation { get; set; }
        public ICollection<ConversationUtilisateur> Participants { get; set; }
        public ICollection<HistoriqueMessages> Messages { get; set; }
    }
}
