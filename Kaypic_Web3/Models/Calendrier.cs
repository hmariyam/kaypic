using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kaypic_Web3.Models
{
    public class Calendrier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire.")]
        public string Titre { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La date est obligatoire.")]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? Heure { get; set; }

        [Required(ErrorMessage = "Le lieu est obligatoire.")]
        public string Lieu { get; set; }

        [Required(ErrorMessage = "Le type est obligatoire.")]
        public Type type { get; set; }

        public int Equipeid { get; set; }
        public Equipe Equipe { get; set; }
        public int Utilisateurid { get; set; }
        public Utilisateur utilisateur { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}