using Kaypic_Web3.Data;
using Kaypic_Web3.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kaypic_Web3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly MainDbContext _context;
        public ApiController(MainDbContext context) { _context = context; }

        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            return Ok("STATUS : OK");
        }


        [HttpGet("equipe")]
        public IActionResult GetAllEquipes()
        {
            var equipes =  _context.Equipes.ToList();
            return Ok(equipes);
        }

        [HttpGet("equipe/nom/{nom}")]
        public IActionResult GetEquipeByName(string nom)
        {
            var equipe = _context.Equipes
                .FirstOrDefault(e => e.Nom == nom);
            if (equipe == null)
                return NotFound("Aucune équipe trouvée");
            return Ok(equipe);
        }


        [HttpGet("equipe/id/{id}")]
        public IActionResult GetEquipeById(int id)
        {
            var equipe = _context.Equipes
                .FirstOrDefault(e => e.Id == id);
            if (equipe == null)
                return NotFound("Aucune équipe trouvée");
            return Ok(equipe);
        }

        [HttpGet("coach/{nom}")]
        public IActionResult GetCoachByName(string nom)
        {
            var coach = _context.Utilisateurs
                .Where(e => (e.Nom == nom || e.Prenom == nom) && e.Role == Models.TypeUtilisateur.Coach)
                .Select(e => new UserDTO
                {
                    Id = e.Id,
                    Nom = e.Nom,
                    Prenom = e.Prenom,
                    Email = e.Email,
                    Role = "coach"
                })
                .FirstOrDefault();
            if (coach == null)
                return NotFound("Aucune coach trouvée");
            return Ok(coach);
        }

        [HttpGet("annonce/{equipeID}")]
        public IActionResult GetAnnonceByEquipeId(int equipeID)
        {
            var events = _context.Annonces.Where(a => a.Equipeid == equipeID).ToList();
            if (events == null || events.Count == 0)
            {
                return NotFound("Aucune annonce trouvée pour cette équipe");
            }
            return Ok(events);
        }

        [HttpGet("evenement/{equipeID}")]
        public IActionResult GetEventByEquipeId(int equipeID)
        {
            var events = _context.Calendriers.Where(a => a.Equipeid == equipeID).ToList();
            if (events == null || events.Count == 0)
            {
                return NotFound("Aucune  trouvée pour cette équipe");
            }
            return Ok(events);
        }
    }
}
