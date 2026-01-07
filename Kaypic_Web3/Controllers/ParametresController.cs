using Kaypic_Web3.Data;
using Kaypic_Web3.Models;
using Kaypic_Web3.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kaypic_Web3.Controllers
{
    public class ParametresController : Controller
    {

        private readonly MainDbContext _context;
        private readonly ISMSSenderService _smsSenderService;


        public ParametresController(MainDbContext context, ISMSSenderService smsSenderService)
        {
            _context = context;
            _smsSenderService = smsSenderService;
        }

        //Icon Parametres LayoutHome
        public IActionResult Parametres()
        {
            var roleValue = HttpContext.Session.GetInt32("RoleUtilisateur");

            if (!roleValue.HasValue)
            {
                return RedirectToAction("Index", "Home");

            }

            return (TypeUtilisateur)roleValue.Value switch
            {
                TypeUtilisateur.Parent => RedirectToAction("ParentParametres", "Parent"),
                TypeUtilisateur.Coach => RedirectToAction("CoachParametres", "Coach"),
                TypeUtilisateur.Joueur => RedirectToAction("JoueurParametres", "Joueur"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // Méthode de suppression d'un compte
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var roleValue = HttpContext.Session.GetInt32("RoleUtilisateur");
            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var currentUser = _context.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);
            var currentUserDeletion = currentUser.DeletionScheduledAt;

            if (currentUser == null)
                return NotFound();

            currentUser.DeletionScheduledAt = DateTime.Now.AddDays(30);
            _context.Utilisateurs.Update(currentUser);

            await _context.SaveChangesAsync();

            TempData["AccountDeletionScheduled"] = true;

            if(currentUser.DeletionScheduledAt != null)
            {
                TempData["DeletionSchedule"] = currentUserDeletion;
            } else
            {
                TempData["DeletionSchedule"] = currentUser.DeletionScheduledAt?.ToUniversalTime().ToString("o");
            }

            // Envoie du message SMS
            if (!string.IsNullOrEmpty(currentUser.Telephone))
            {
                string message = $"Bonjour {currentUser.Prenom}, votre compte sera supprimé dans 30 jours (le {currentUser.DeletionScheduledAt:dd/MM/yyyy}).";
                await _smsSenderService.SendSmsAsync(currentUser.Telephone, message);
            }

            return (TypeUtilisateur)roleValue.Value switch
            {
                TypeUtilisateur.Parent => RedirectToAction("ParentParametres", "Parent"),
                TypeUtilisateur.Coach => RedirectToAction("CoachParametres", "Coach"),
                TypeUtilisateur.Joueur => RedirectToAction("JoueurParametres", "Joueur"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
