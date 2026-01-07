using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaypic_Web3.Models
{
    public class ConversationUtilisateur
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public int Id_utilisateur { get; set; }
        public DateTime ajouter_a { get; set; }
    }
}