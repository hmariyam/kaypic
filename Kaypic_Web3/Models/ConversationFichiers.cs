using Kaypic_Web3.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaypic_Web3.Models
{
    public class ConversationFichiers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public TypeFichier TypeFichier { get; set; }
        public int CreerParIdUtilisateur { get; set; }
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileUrl { get; set; }
    }
}