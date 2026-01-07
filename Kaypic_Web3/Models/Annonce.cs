using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaypic_Web3.Models
{
    public class Annonce
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        public string Titre { get; set; }
        [Required(ErrorMessage = "La description est obligatoire.")]
        public string Description { get; set; }
        public Priorite priorite { get; set; }
        public Boolean Pinned { get; set; }
        public int Equipeid { get; set; }
        public Equipe Equipe { get; set; }

    }
}