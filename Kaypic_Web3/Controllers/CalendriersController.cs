using Kaypic_Web3.Data;
using Kaypic_Web3.Models;
using Kaypic_Web3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaypic_Web3.Controllers
{
    public class CalendriersController : Controller
    {
        private readonly MainDbContext _context;

        public CalendriersController(MainDbContext context)
        {
            _context = context;
        }

        // GET: Calendriers
        public async Task<IActionResult> Index()
        {
            var mainDbContext = await _context.Calendriers.Include(c => c.Equipe).Include(c => c.utilisateur).ToListAsync();
            return View(mainDbContext);
        }

        // GET: Calendriers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calendrier = await _context.Calendriers
                .Include(c => c.Equipe)
                .Include(c => c.utilisateur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (calendrier == null)
            {
                return NotFound();
            }

            return View(calendrier);
        }

        // GET: Calendriers/Create
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var user = _context.Utilisateurs.Include(u => u.Equipe).FirstOrDefault(u => u.CustomId == userId);

            if (user != null)
            {
                ViewBag.UserEquipeName = user.Equipe?.Nom ?? "Équipe non assignée";
                ViewBag.UserEquipeId = user.EquipeId;
            }

            return View();
        }

        // POST: Calendriers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titre,Description,Date,Heure,Lieu,type,Equipeid")] Calendrier calendrier)
        {
            // Enlever ces deux champs pour effectuer la validation
            ModelState.Remove("Equipe");
            ModelState.Remove("utilisateur");

            // En premier pour que le système défini l'id de l'utilisateur dès le départ
            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            calendrier.Utilisateurid = userId.Value;

            // Chercher le rôle de l'utilisateur afin de retourner la bonne page
            var role = HttpContext.Session.GetInt32("RoleUtilisateur");

            if (!ModelState.IsValid)
            {

                ViewData["Equipeid"] = new SelectList(_context.Equipes, "Id", "Nom", calendrier.Equipeid);

                var user = _context.Utilisateurs.Include(u => u.Equipe).FirstOrDefault(u => u.CustomId == userId);

                if (user != null)
                {
                    ViewBag.UserEquipeName = user.Equipe?.Nom ?? "Équipe non assignée";
                    ViewBag.UserEquipeId = user.EquipeId;
                }

                ViewBag.ErrorMessage = "Veuillez compléter le formulaire avant de soumettre.";
                return View(calendrier);
            }

            ViewData["Equipeid"] = new SelectList(_context.Equipes, "Id", "Nom", calendrier.Equipeid);

            _context.Add(calendrier);
            await _context.SaveChangesAsync();

            return role switch
            {
                (int)TypeUtilisateur.Parent => RedirectToAction("ParentCalendrier", "Home"),
                (int)TypeUtilisateur.Coach => RedirectToAction("CoachCalendrier", "Home"),
                (int)TypeUtilisateur.Joueur => RedirectToAction("JoueurCalendrier", "Home"),
                _ => RedirectToAction("Index", "Home"),
            };
        }

        // GET: Calendriers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calendrier = await _context.Calendriers.FindAsync(id);
            if (calendrier == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var user = _context.Utilisateurs.Include(u => u.Equipe).FirstOrDefault(u => u.CustomId == userId);

            if (user != null)
            {
                ViewBag.UserEquipeName = user.Equipe?.Nom ?? "Équipe non assignée";
                ViewBag.UserEquipeId = user.EquipeId;
            }

            ViewData["Equipeid"] = new SelectList(_context.Equipes, "Id", "Nom", calendrier.Equipeid);
            return View(calendrier);
        }

        // POST: Calendriers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,Date,Heure,Lieu,type,Equipeid")] Calendrier calendrier)
        {

            if (id != calendrier.Id)
            {
                return NotFound();
            }

            try
            {
                // En premier pour que le système défini l'id de l'utilisateur dès le départ
                var userId = HttpContext.Session.GetInt32("UtilisateurID");
                calendrier.Utilisateurid = userId.Value;

                _context.Update(calendrier);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CalendrierExists(calendrier.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            ViewData["Equipeid"] = new SelectList(_context.Equipes, "Id", "Nom", calendrier.Equipeid);
            var role = HttpContext.Session.GetInt32("RoleUtilisateur");

            return role switch
            {
                (int)TypeUtilisateur.Parent => RedirectToAction("ParentCalendrier", "Home"),
                (int)TypeUtilisateur.Coach => RedirectToAction("CoachCalendrier", "Home"),
                (int)TypeUtilisateur.Joueur => RedirectToAction("JoueurCalendrier", "Home"),
                _ => RedirectToAction("Index", "Home"),
            };
        }

        // GET: Calendriers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calendrier = await _context.Calendriers
                .Include(c => c.Equipe)
                .Include(c => c.utilisateur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (calendrier == null)
            {
                return NotFound();
            }

            return View(calendrier);
        }

        // POST: Calendriers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calendrier = await _context.Calendriers.FindAsync(id);
            if (calendrier != null)
            {
                _context.Calendriers.Remove(calendrier);
            }

            await _context.SaveChangesAsync();

            var role = HttpContext.Session.GetInt32("RoleUtilisateur");

            return role switch
            {
                (int)TypeUtilisateur.Parent => RedirectToAction("ParentCalendrier", "Home"),
                (int)TypeUtilisateur.Coach => RedirectToAction("CoachCalendrier", "Home"),
                (int)TypeUtilisateur.Joueur => RedirectToAction("JoueurCalendrier", "Home"),
                _ => RedirectToAction("Index", "Home"),
            };
        }

        private bool CalendrierExists(int id)
        {
            return _context.Calendriers.Any(e => e.Id == id);
        }


        //pour répondre au question AI - Parent
        [HttpGet]
        public JsonResult GetNextEvent()
        {
            var userId = HttpContext.Session.GetInt32("UtilisateurID");

            if (userId == null)
                return Json("Impossible d'identifier l'utilisateur.");

            var now = DateTime.Now;

            var nextEvent = _context.Calendriers
                .Where(e => e.Utilisateurid == userId && e.Date >= now)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Heure)
                .FirstOrDefault();

            if (nextEvent == null)
                return Json("Vous n'avez aucun événement à venir.");

            string heureStr = nextEvent.Heure?.ToString(@"hh\:mm") ?? "À toute heure";

            string message =
                $"Votre prochain événement est : {nextEvent.Titre} — le {nextEvent.Date:dd MMMM yyyy} à {heureStr}.";

            return Json(message);
        }

    }
}
