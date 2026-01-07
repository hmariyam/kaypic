using Kaypic_Web3.Data;
using Kaypic_Web3.Models;
using Kaypic_Web3.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace GestionDesSessionsCookies.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly MainDbContext _context; 
        private readonly ISMSSenderService _smsSenderService;

        public AccountController(SignInManager<Utilisateur> signInManager, UserManager<Utilisateur> userManager, ISMSSenderService smsSenderService, MainDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager; 
            _smsSenderService = smsSenderService;
            _context = context;
        }

        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(string email, string mdp)
        {
            //trouver les utilisateurs dans la bd
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Courriel == email);

            if (user != null && !string.IsNullOrEmpty(user.Mdp))
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(mdp, user.Mdp);

                if (isPasswordValid)
                {
                    if (user.IsDeleted)
                    {
                        TempData["AlertMessage"] = "Compte supprimé. Vous ne pouvez plus vous connecter.";
                        return RedirectToAction("Index", "Home");
                    }

                    HttpContext.Session.SetString("CourrielUtilisateur", user.Courriel);
                    HttpContext.Session.SetInt32("RoleUtilisateur", (int)user.Role);
                    HttpContext.Session.SetString("NomUtilisateur", user.Prenom);
                    HttpContext.Session.SetInt32("UtilisateurID", user.CustomId);
                    HttpContext.Session.SetInt32("EquipeUtilisateurId", user.EquipeId);

                    // Générer un code aléatoire
                    var random = new Random();
                    int code = random.Next(100000, 999999);

                    HttpContext.Session.SetString("MfaCode", code.ToString());

                    // Envoyer le code par SMS
                    await _smsSenderService.SendSmsAsync(user.Telephone,
                        $"Votre code de vérification est : {code}");

                    return RedirectToAction("VerifyMFA", "Account");
                }
            }

            ViewBag.ErrorMessage = "Identifiants incorrects";
            return View();
        }


        [HttpGet]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }


        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Courriel == email);

            if (user != null)
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser != null)
                {
                    var logins = await _userManager.GetLoginsAsync(identityUser);
                    if (!logins.Any(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey))
                        await _userManager.AddLoginAsync(identityUser, info);

                    await _signInManager.SignInAsync(identityUser, isPersistent: false);
                }

                HttpContext.Session.SetString("CourrielUtilisateur", user.Courriel);
                HttpContext.Session.SetInt32("RoleUtilisateur", (int)user.Role);
                HttpContext.Session.SetString("NomUtilisateur", user.Prenom);
                HttpContext.Session.SetInt32("UtilisateurID", user.CustomId);
                HttpContext.Session.SetInt32("EquipeUtilisateurId", user.EquipeId);


                switch (user.Role)
                {
                    case TypeUtilisateur.Joueur:
                        return RedirectToAction("JoueurHome", "Home");
                    case TypeUtilisateur.Coach:
                        return RedirectToAction("CoachHome", "Home");
                    case TypeUtilisateur.Parent:
                        return RedirectToAction("ParentHome", "Home");
                    default:
                        return RedirectToAction("Index", "Home");
                }
            }

            // Ereur
            ViewBag.ErrorMessage = "Impossible de login via Google.";
            return RedirectToAction("Login");
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["LogoutMessage"] = "Vous avez fermé votre session, à bientôt !";
            return RedirectToAction("Index", "Home");
        }


        // Méthode qui nous retourne vers la page pour l'authentification multi-factor
        public IActionResult VerifyMFA()
        {
            string code = HttpContext.Session.GetString("MfaCode");

            if (code == null)
                return RedirectToAction("Login");

            ViewBag.GeneratedCode = code;
            return View();
        }

        // Méthode qui vérifie le code entrée et le rôle de l'utilisateur
        [HttpPost]
        public IActionResult VerifyMFA(string enteredCode)
        {
            string expectedCode = HttpContext.Session.GetString("MfaCode");

            if (expectedCode == null)
                return RedirectToAction("Login");

            if (enteredCode == expectedCode)
            {

                HttpContext.Session.Remove("MfaCode");

                var role = HttpContext.Session.GetInt32("RoleUtilisateur");
                return role switch
                {
                    (int)TypeUtilisateur.Parent => RedirectToAction("ParentHome", "Home"),
                    (int)TypeUtilisateur.Coach => RedirectToAction("CoachHome", "Home"),
                    (int)TypeUtilisateur.Joueur => RedirectToAction("JoueurHome", "Home"),
                    _ => RedirectToAction("Index", "Home"),
                };
            }

            ViewBag.ErrorMessage = "Code incorrect. Veuillez réessayer.";
            ViewBag.GeneratedCode = expectedCode;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.OldMDP) || string.IsNullOrEmpty(model.NewMDP))
                return BadRequest(new { message = "Veuillez remplir tous les champs." });

             var userId = HttpContext.Session.GetInt32("UtilisateurID");
            // if (userId == null)
            // return Unauthorized(new { message = "Session expirée." });

            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.CustomId == userId);
            //  if (user == null)
            //   return NotFound(new { message = "Utilisateur introuvable." });

            // Vérifie l'ancien
            bool validOld = BCrypt.Net.BCrypt.Verify(model.OldMDP, user.Mdp);
            if (!validOld)
                return BadRequest(new { message = "Ancien mot de passe incorrect ❓" });

            // Met à jour 
            user.Mdp = BCrypt.Net.BCrypt.HashPassword(model.NewMDP);
            _context.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Mot de passe mis à jour avec succès ✅" });
        }

        public class ChangePasswordDto
        {
            public string OldMDP { get; set; }
            public string NewMDP { get; set; }
        }
    }
}