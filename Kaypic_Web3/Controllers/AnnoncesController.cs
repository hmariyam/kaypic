using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kaypic_Web3.Data;
using Kaypic_Web3.Models;

namespace Kaypic_Web3.Controllers
{
    public class AnnoncesController : Controller
    {
        private readonly MainDbContext _context;

        public AnnoncesController(MainDbContext context)
        {
            _context = context;
        }

        // GET: Annonces
        public async Task<IActionResult> Index()
        {
            var mainDbContext = _context.Annonces.Include(a => a.Equipe);
            return View(await mainDbContext.ToListAsync());
        }

        // GET: Annonces/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annonce = await _context.Annonces
                .Include(a => a.Equipe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (annonce == null)
            {
                return NotFound();
            }

            return View(annonce);
        }

        // GET: Annonces/Create
        public IActionResult Create()
        {
            ViewData["Equipeid"] = new SelectList(_context.Equipes, "Id", "Id");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(string Titre, string Description, Priorite priorite, bool Pinned)
        {
            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var utilisateur = _context.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);
            var annonce = new Annonce
            {
                Titre = Titre,
                Description = Description,
                priorite = priorite,
                Pinned = Pinned,
                Equipeid = utilisateur.EquipeId
            };

            if (ModelState.IsValid)
            {
                _context.Add(annonce);
                await _context.SaveChangesAsync();
                return RedirectToAction("CoachAnnonce", "Home");
            }
            ViewData["Equipeid"] = new SelectList(_context.Equipes, "Id", "Id", annonce.Equipeid);
            return View(annonce);
        }

        // GET: Annonces/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annonce = await _context.Annonces.FindAsync(id);
            if (annonce == null)
            {
                return NotFound();
            }
            ViewData["Equipeid"] = new SelectList(_context.Equipes, "Id", "Id", annonce.Equipeid);
            return View(annonce);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Titre, string Description, Priorite priorite, bool Pinned)
        {
            Annonce annonce = _context.Annonces.Find(id);
            annonce.Titre = Titre;
            annonce.Description = Description;
            annonce.priorite = priorite;
            annonce.Pinned = Pinned;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(annonce);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnonceExists(annonce.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("CoachAnnonce", "Home");
            }
            ViewData["Equipeid"] = new SelectList(_context.Equipes, "Id", "Id", annonce.Equipeid);
            return View(annonce);
        }

        // GET: Annonces/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annonce = await _context.Annonces
                .Include(a => a.Equipe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (annonce == null)
            {
                return NotFound();
            }

            return View(annonce);
        }

        // POST: Annonces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var annonce = await _context.Annonces.FindAsync(id);
            if (annonce != null)
            {
                _context.Annonces.Remove(annonce);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("CoachAnnonce", "Home");
        }

        private bool AnnonceExists(int id)
        {
            return _context.Annonces.Any(e => e.Id == id);
        }
    }
}
