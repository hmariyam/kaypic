using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaypic_Web3.Models
{
    public class Utilisateur : IdentityUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomId { get; set; }
        public TypeUtilisateur Role { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public int Age { get; set; }
        public string Courriel { get; set; }
        public string Telephone { get; set; }
        public string? Mdp { get; set; }
        public int EquipeId { get; set; }
        public Equipe Equipe { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletionScheduledAt { get; set; }
    }
}