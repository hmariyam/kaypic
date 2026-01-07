using Kaypic_Web3.Data;
using Kaypic_Web3.Models;
using Kaypic_Web3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kaypic_Web3.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MainDbContext _db;
        private readonly MessagingDbContext _msg;
        private readonly ISMSSenderService _smsSenderService;

        public HomeController(ILogger<HomeController> logger, MainDbContext db, MessagingDbContext dbMsg, ISMSSenderService smsSenderService)
        {
            _logger = logger;
            _db = db;
            _msg = dbMsg;
            _smsSenderService = smsSenderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Partie Coach
        [Route("home/CoachHome")]
        public IActionResult CoachHome()
        {
            return View();
        }

        [Route("Coach/CoachMessage")]
        public IActionResult CoachMessage()
        {
            ViewData["PartialName"] = "Components/Coach/CoachMessage";
            var userId = HttpContext.Session.GetInt32("UtilisateurID");

            var equipeId = _db.Utilisateurs
                .Where(u => u.CustomId == userId)
                .Select(u => u.EquipeId)
                .FirstOrDefault();
            CreerConversations(equipeId);

            var currentUser = _db.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);

            if (currentUser == null) return NotFound();

            var conversations = _msg.Conversations
                .Include(c => c.Participants)
                .ToList()
                .Where(c => c.Participants.Any(p => p.Id_utilisateur == userId))
                .Where(c =>
                {
                    if (c.Participants.Count != 2) return true;

                    var otherUser = c.Participants
                        .Select(p => _db.Utilisateurs.FirstOrDefault(u => u.CustomId == p.Id_utilisateur))
                        .FirstOrDefault(u => u.CustomId != currentUser.CustomId);

                    if (otherUser == null) return false;

                    if (currentUser.Role == TypeUtilisateur.Coach && otherUser.Age < 18)
                        return false;

                    if (otherUser.EquipeId != currentUser.EquipeId)
                        return false;

                    return true;
                })
                .ToList();

            foreach (var conversation in conversations)
            {

                if (conversation.Participants == null || conversation.Participants.Count == 0)
                    continue;

                if (conversation.Participants.Count == 2)
                {
                    var destinataireId = conversation.Participants
                        .FirstOrDefault(p => p.Id_utilisateur != userId)?.Id_utilisateur;
                    if (destinataireId != null)
                    {
                        var destinataire = _db.Utilisateurs
                            .FirstOrDefault(u => u.CustomId == destinataireId);
                        conversation.Titre = $"{destinataire?.Prenom} {destinataire?.Nom}";
                    }
                }
            }

            var vm = new HomeVM
            {
                Conversations = conversations,
                Current = null
            };
            return View("CoachHome", vm);
        }

        //pour ouvrir une conversation et afficher l'historique
        [Route("Coach/Conversation/{id:int}")]
        public IActionResult Conversation(int id)
        {
            ViewData["PartialName"] = "Components/Coach/CoachMessage";
            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var currentUser = _db.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);

            if (currentUser == null) return NotFound();

            var conversations = _msg.Conversations
                .Include(c => c.Participants)
                .ToList()
                .Where(c => c.Participants.Any(p => p.Id_utilisateur == userId))
                .Where(c =>
                {
                    if (c.Participants.Count != 2) return true;

                    var otherUser = c.Participants
                        .Select(p => _db.Utilisateurs.FirstOrDefault(u => u.CustomId == p.Id_utilisateur))
                        .FirstOrDefault(u => u.CustomId != currentUser.CustomId);

                    if (otherUser == null) return false;

                    if (currentUser.Role == TypeUtilisateur.Coach && otherUser.Age < 18)
                        return false;

                    if (otherUser.EquipeId != currentUser.EquipeId)
                        return false;

                    return true;
                })
                .ToList();

            foreach (var c in conversations)
            {
                if (c.Participants == null || c.Participants.Count == 0)
                    continue;

                if (c.Participants.Count == 2)
                {
                    var destinataireId = c.Participants.FirstOrDefault(p => p.Id_utilisateur != userId)?.Id_utilisateur;
                    if (destinataireId != null)
                    {
                        var u = _db.Utilisateurs.FirstOrDefault(x => x.CustomId == destinataireId);
                        c.Titre = $"{u?.Prenom} {u?.Nom}";
                    }
                }
            }

            var conv = _msg.Conversations
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Reactions)
                .Include(c => c.Participants)
                .FirstOrDefault(c => c.Id == id);

            if (conv is null) return NotFound();

            string titre;

            if (conv.Participants.Count > 2)
            {
                titre = conv.Titre;
            }
            else
            {
                var destId = conv.Participants.FirstOrDefault(p => p.Id_utilisateur != userId)?.Id_utilisateur;
                titre = "Conversation";
                if (destId != null)
                {
                    var u = _db.Utilisateurs.FirstOrDefault(x => x.CustomId == destId);
                    if (u != null) titre = $"{u.Prenom} {u.Nom}";
                }
            }

            var current = new ConversationVM
            {
                ConversationId = conv.Id,
                CurrentUserId = userId.Value,
                CurrentUserIdReaction = currentUser.CustomId,
                CurrentUserName = currentUser.Nom + " " + currentUser.Prenom,
                Titre = titre,
                HeaderDate = conv.Messages.OrderBy(m => m.DateEnvoi).FirstOrDefault()?.DateEnvoi,
                Messages = conv.Messages.OrderBy(m => m.DateEnvoi).ToList()
            };

            var vm = new HomeVM
            {
                Conversations = conversations,
                Current = current
            };

            return View("CoachHome", vm);
        }
        
        // Méthode qui cherche les utilisateurs selon le filtrage
        [HttpGet]
        public async Task<IActionResult> GetAllowedUsers()
        {
            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var utilisateur = _db.Utilisateurs.Include(u => u.Equipe).FirstOrDefault(u => u.CustomId == userId);

            if (userId == null) return Json(new object[0]);

            var teamId = await _db.Users
                                  .Where(c => c.CustomId == userId)
                                  .Select(c => c.EquipeId)
                                  .FirstOrDefaultAsync();

            var users = await _db.Users
                                 .Where(u => u.EquipeId == teamId && u.CustomId != userId)
                                 .Select(u => new { u.CustomId, u.Nom, u.Prenom })
                                 .ToListAsync();

            // Filtrage pour que les joueurs puisse seulement ajouter des utilisateurs de type joueurs à un groupe
            var utilisateurType = await _db.Utilisateurs.Where(c => c.CustomId == userId).Select(c => c.Role).FirstOrDefaultAsync();
            if (utilisateurType == TypeUtilisateur.Joueur)
            {
                bool adulte = utilisateur.Age >= 18;

                users = await _db.Users
                    .Where(u =>
                        u.EquipeId == utilisateur.EquipeId &&
                        u.CustomId != utilisateur.CustomId &&
                        u.Role == TypeUtilisateur.Joueur &&
                        (
                            (adulte && u.Age >= 18) ||
                            (!adulte && u.Age < 18)
                        )
                    )
                    .Select(u => new { u.CustomId, u.Nom, u.Prenom })
                    .ToListAsync();
            }

            return Json(users);
        }

        [Route("Coach/CoachCalendrier")]
        public async Task<IActionResult> CoachCalendrier()
        {
            ViewData["PartialName"] = "Components/Coach/CoachCalendrier";

            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var currentUser = _db.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);

            var calendriers = await _db.Calendriers
                .Include(c => c.Equipe)
                .Include(c => c.utilisateur)
                .Where(c => c.Utilisateurid == userId)
                .ToListAsync();

            foreach (var calendrier in calendriers)
            {
                var tempsAvantEvent = calendrier.Date - DateTime.Now;

                if (tempsAvantEvent.TotalHours <= 24 && tempsAvantEvent.TotalHours > 0)
                {
                    string message = $"Bonjour {currentUser.Prenom}, vous avez un événement prévu le {calendrier.Date:dd/MM/yyyy HH:mm}.";
                    await _smsSenderService.SendSmsAsync(currentUser.Telephone, message);
                }
            }

            var vm = new HomeVM
            {
                Calendriers = calendriers,
            };
            return View("CoachHome", vm);
        }

        [Route("Coach/CoachAnnonce")]
        public IActionResult CoachAnnonce()
        {
            ViewData["PartialName"] = "Components/Coach/CoachAnnonce";

            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var utilisateur = _db.Utilisateurs.Include(u => u.Equipe).FirstOrDefault(u => u.CustomId == userId);
            var annonces = _db.Annonces.Where(a => a.Equipeid == utilisateur.EquipeId).ToList();
            var vm = new HomeVM
            {
                Annonces = annonces
            };
            return View("CoachHome", vm);
        }

        [Route("Coach/CoachParametres")]
        public IActionResult CoachParametres()
        {
            ViewData["PartialName"] = "Components/Coach/CoachParametres";
            return View("CoachHome");
        }


        //Partie Joueur
        [Route("home/JoueurHome")]
        public IActionResult JoueurHome()
        {
            return View();
        }

        [Route("Joueur/JoueurMessage")]
        public IActionResult JoueurMessage()
        {
            ViewData["PartialName"] = "Components/Joueur/JoueurMessage";
            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var utilisateur = _db.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);
            bool isAdult = utilisateur.Age >= 18;

            var equipeId = _db.Utilisateurs
                .Where(u => u.CustomId == userId)
                .Select(u => u.EquipeId)
                .FirstOrDefault();
            CreerConversations(equipeId);

            var conversations = _msg.Conversations
                .Include(c => c.Participants)
                .Where(c => c.Participants.Any(p => p.Id_utilisateur == userId))
                .ToList()
                .Where(c => c.Participants.All(p =>
                {
                    var u = _db.Utilisateurs.FirstOrDefault(x => x.CustomId == p.Id_utilisateur);
                    if (u == null) return false;
                    bool ageOk = (u.Age >= 18 && utilisateur.Age >= 18) || (u.Age < 18 && utilisateur.Age < 18);
                    bool sameTeam = u.EquipeId == utilisateur.EquipeId;

                    return ageOk && sameTeam;
                }))
                .ToList();

            foreach (var conversation in conversations)
            {

                if (conversation.Participants == null || conversation.Participants.Count == 0)
                    continue;

                if (conversation.Participants.Count == 2)
                {
                    var destinataireId = conversation.Participants
                        .FirstOrDefault(p => p.Id_utilisateur != userId)?.Id_utilisateur;
                    if (destinataireId != null)
                    {
                        var destinataire = _db.Utilisateurs
                            .FirstOrDefault(u => u.CustomId == destinataireId);
                        conversation.Titre = $"{destinataire?.Prenom} {destinataire?.Nom}";
                    }
                }
            }

            var vm = new HomeVM
            {
                Conversations = conversations,
                Current = null
            };
            return View("JoueurHome", vm);
        }

        [Route("Joueur/Conversation/{id:int}")]
        public IActionResult ConversationJoueur(int id)
        {
            ViewData["PartialName"] = "Components/Joueur/JoueurMessage";
            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var utilisateur = _db.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);
            bool isAdult = utilisateur.Age >= 18;

            var conversations = _msg.Conversations
                .Include(c => c.Participants)
                .Where(c => c.Participants.Any(p => p.Id_utilisateur == userId))
                .ToList()
                .Where(c => c.Participants.All(p =>
                {
                    var u = _db.Utilisateurs.FirstOrDefault(x => x.CustomId == p.Id_utilisateur);
                    if (u == null) return false; 
                    bool ageOk = (u.Age >= 18 && utilisateur.Age >= 18) || (u.Age < 18 && utilisateur.Age < 18);
                    bool sameTeam = u.EquipeId == utilisateur.EquipeId;

                    return ageOk && sameTeam;
                }))
                .ToList();

            foreach (var c in conversations)
            {
                if (c.Participants == null || c.Participants.Count == 0)
                    continue;

                if (c.Participants.Count == 2)
                {
                    var destinataireId = c.Participants.FirstOrDefault(p => p.Id_utilisateur != userId)?.Id_utilisateur;
                    if (destinataireId != null)
                    {
                        var u = _db.Utilisateurs.FirstOrDefault(x => x.CustomId == destinataireId);
                        c.Titre = $"{u?.Prenom} {u?.Nom}";
                    }
                }
            }

            var conv = _msg.Conversations
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Reactions)
                .Include(c => c.Participants)
                .FirstOrDefault(c => c.Id == id);

            if (conv is null) return NotFound();

            string titre;
            if (conv.Participants.Count > 2)
            {
                titre = conv.Titre;
            }
            else
            {
                var destId = conv.Participants.FirstOrDefault(p => p.Id_utilisateur != userId)?.Id_utilisateur;
                titre = "Conversation";
                if (destId != null)
                {
                    var u = _db.Utilisateurs.FirstOrDefault(x => x.CustomId == destId);
                    if (u != null) titre = $"{u.Prenom} {u.Nom}";
                }
            }

            var current = new ConversationVM
            {
                ConversationId = conv.Id,
                CurrentUserId = userId.Value,
                CurrentUserIdReaction = utilisateur.CustomId,
                CurrentUserName = utilisateur.Nom + " " + utilisateur.Prenom,
                Titre = titre,
                HeaderDate = conv.Messages.OrderBy(m => m.DateEnvoi).FirstOrDefault()?.DateEnvoi,
                Messages = conv.Messages.OrderBy(m => m.DateEnvoi).ToList()
            };

            var vm = new HomeVM
            {
                Conversations = conversations,
                Current = current
            };
            return View("JoueurHome", vm);
        }


        [Route("Joueur/JoueurAnnonce")]
        public IActionResult JoueurAnnonce()
        {
            ViewData["PartialName"] = "Components/Joueur/JoueurAnnonce";

            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var utilisateur = _db.Utilisateurs.Include(u => u.Equipe).FirstOrDefault(u => u.CustomId == userId);
            var annonces = _db.Annonces.Where(a => a.Equipeid == utilisateur.EquipeId).ToList();
            var vm = new HomeVM
            {
                Annonces = annonces
            };
            return View("JoueurHome", vm);
        }

        [Route("Joueur/JoueurCalendrier")]
        public async Task<IActionResult> JoueurCalendrier()
        {
            ViewData["PartialName"] = "Components/Joueur/JoueurCalendrier";

            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var currentUser = _db.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);
            var equipeId = HttpContext.Session.GetInt32("EquipeUtilisateurId");

            var calendriers = await _db.Calendriers
                .Include(c => c.Equipe)
                .Include(c => c.utilisateur)
                .Where(c => c.Utilisateurid == userId || c.utilisateur.Role == TypeUtilisateur.Coach && c.Equipeid == equipeId)
                .ToListAsync();

            foreach (var calendrier in calendriers)
            {
                var tempsAvantEvent = calendrier.Date - DateTime.Now;

                if (tempsAvantEvent.TotalHours <= 24 && tempsAvantEvent.TotalHours > 0)
                {
                    string message = $"Bonjour {currentUser.Prenom}, vous avez un événement prévu le {calendrier.Date:dd/MM/yyyy HH:mm}.";
                    await _smsSenderService.SendSmsAsync(currentUser.Telephone, message);
                }
            }

            var vm = new HomeVM
            {
                Calendriers = calendriers,
            };
            return View("JoueurHome", vm);
        }

        [Route("Joueur/JoueurParametres")]
        public IActionResult JoueurParametres()
        {
            ViewData["PartialName"] = "Components/Joueur/JoueurParametres";
            return View("JoueurHome");
        }


        //Partie Parent
        [Route("home/ParentHome")]
        public IActionResult ParentHome()
        {
            return View();
        }

        [Route("Parent/ParentMessage")]
        public IActionResult ParentMessage()
        {
            ViewData["PartialName"] = "Components/Parent/ParentMessage";

            var userId = HttpContext.Session.GetInt32("UtilisateurID");

            var equipeId = _db.Utilisateurs
                .Where(u => u.CustomId == userId)
                .Select(u => u.EquipeId)
                .FirstOrDefault();
            CreerConversations(equipeId);

            var currentUser = _db.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);
            if (currentUser == null) return NotFound();

            var conversations = _msg.Conversations
                .Include(c => c.Participants)
                .ToList()
                .Where(c => c.Participants.Any(p => p.Id_utilisateur == userId))
                .Where(c =>
                {
                    if (c.Participants.Count != 2) return true;

                    var otherUser = c.Participants
                        .Select(p => _db.Utilisateurs.FirstOrDefault(u => u.CustomId == p.Id_utilisateur))
                        .FirstOrDefault(u => u.CustomId != currentUser.CustomId);

                    if (otherUser == null) return false;

                    if (currentUser.Role == TypeUtilisateur.Parent && otherUser.Age < 18)
                        return false;

                    if (otherUser.EquipeId != currentUser.EquipeId)
                        return false;

                    return true;
                })
                .ToList();

            foreach (var conversation in conversations)
            {

                if (conversation.Participants == null || conversation.Participants.Count == 0)
                    continue;

                if (conversation.Participants.Count == 2)
                {
                    var destinataireId = conversation.Participants
                        .FirstOrDefault(p => p.Id_utilisateur != userId)?.Id_utilisateur;
                    if (destinataireId != null)
                    {
                        var destinataire = _db.Utilisateurs
                            .FirstOrDefault(u => u.CustomId == destinataireId);
                        conversation.Titre = $"{destinataire?.Prenom} {destinataire?.Nom}";
                    }
                }
            }

            var vm = new HomeVM
            {
                Conversations = conversations,
                Current = null
            };
            return View("ParentHome", vm);
        }

        [Route("Parent/Conversation/{id:int}")]
        public IActionResult ConversationParent(int id)
        {
            ViewData["PartialName"] = "Components/Parent/ParentMessage";
            var userId = HttpContext.Session.GetInt32("UtilisateurID");

            var currentUser = _db.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);
            if (currentUser == null) return NotFound();

            var conversations = _msg.Conversations
                .Include(c => c.Participants)
                .ToList()
                .Where(c => c.Participants.Any(p => p.Id_utilisateur == userId))
                .Where(c =>
                {
                    if (c.Participants.Count != 2) return true;

                    var otherUser = c.Participants
                        .Select(p => _db.Utilisateurs.FirstOrDefault(u => u.CustomId == p.Id_utilisateur))
                        .FirstOrDefault(u => u.CustomId != currentUser.CustomId);

                    if (otherUser == null) return false;

                    if (currentUser.Role == TypeUtilisateur.Parent && otherUser.Age < 18)
                        return false;

                    if (otherUser.EquipeId != currentUser.EquipeId)
                        return false;

                    return true;
                })
                .ToList();

            foreach (var c in conversations)
            {
                if (c.Participants == null || c.Participants.Count == 0)
                    continue;

                if (c.Participants.Count == 2)
                {
                    var destinataireId = c.Participants.FirstOrDefault(p => p.Id_utilisateur != userId)?.Id_utilisateur;
                    if (destinataireId != null)
                    {
                        var u = _db.Utilisateurs.FirstOrDefault(x => x.CustomId == destinataireId);
                        c.Titre = $"{u?.Prenom} {u?.Nom}";
                    }
                }
            }

            var conv = _msg.Conversations
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Reactions)
                .Include(c => c.Participants)
                .FirstOrDefault(c => c.Id == id);

            if (conv is null) return NotFound();

            string titre;
            if (conv.Participants.Count > 2)
            {
                titre = conv.Titre;
            }
            else
            {
                var destId = conv.Participants.FirstOrDefault(p => p.Id_utilisateur != userId)?.Id_utilisateur;
                titre = "Conversation";
                if (destId != null)
                {
                    var u = _db.Utilisateurs.FirstOrDefault(x => x.CustomId == destId);
                    if (u != null) titre = $"{u.Prenom} {u.Nom}";
                }
            }

            var current = new ConversationVM
            {
                ConversationId = conv.Id,
                CurrentUserId = userId.Value,
                CurrentUserIdReaction = currentUser.CustomId,
                CurrentUserName = currentUser.Nom + " " + currentUser.Prenom,
                Titre = titre,
                HeaderDate = conv.Messages.OrderBy(m => m.DateEnvoi).FirstOrDefault()?.DateEnvoi,
                Messages = conv.Messages.OrderBy(m => m.DateEnvoi).ToList()
            };

            var vm = new HomeVM
            {
                Conversations = conversations,
                Current = current
            };

            return View("ParentHome", vm);
        }


        [Route("Parent/ParentAnnonce")]
        public IActionResult ParentAnnonce()
        {
            ViewData["PartialName"] = "Components/Parent/ParentAnnonce";

            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var utilisateur = _db.Utilisateurs.Include(u => u.Equipe).FirstOrDefault(u => u.CustomId == userId);
            var annonces = _db.Annonces.Where(a => a.Equipeid == utilisateur.EquipeId).ToList();
            var vm = new HomeVM
            {
                Annonces = annonces
            };
            return View("ParentHome", vm);
        }

        [Route("Parent/ParentCalendrier")]
        public async Task<IActionResult> ParentCalendrier()
        {
            ViewData["PartialName"] = "Components/Parent/ParentCalendrier";

            var userId = HttpContext.Session.GetInt32("UtilisateurID");
            var currentUser = _db.Utilisateurs.FirstOrDefault(u => u.CustomId == userId);
            var equipeId = HttpContext.Session.GetInt32("EquipeUtilisateurId");

            var calendriers = await _db.Calendriers
                .Include(c => c.Equipe)
                .Include(c => c.utilisateur)
                .Where(c => c.Utilisateurid == userId || c.utilisateur.Role == TypeUtilisateur.Coach && c.Equipeid == equipeId)
                .ToListAsync();

            foreach (var calendrier in calendriers)
            {
                var tempsAvantEvent = calendrier.Date - DateTime.Now;

                if (tempsAvantEvent.TotalHours <= 24 && tempsAvantEvent.TotalHours > 0)
                {
                    string message = $"Bonjour {currentUser.Prenom}, vous avez un événement prévu le {calendrier.Date:dd/MM/yyyy HH:mm}.";
                    await _smsSenderService.SendSmsAsync(currentUser.Telephone, message);
                }
            }

            var vm = new HomeVM
            {
                Calendriers = calendriers,
            };
            return View("ParentHome", vm);
        }

        [Route("Parent/ParentParametres")]
        public IActionResult ParentParametres()
        {
            ViewData["PartialName"] = "Components/Parent/ParentParametres";
            return View("ParentHome");
        }

        [HttpGet]
        public JsonResult GetCoachs()
        {
            var coachNames = _db.Utilisateurs
                .Where(u => u.Role == TypeUtilisateur.Coach)
                .Select(c => c.Prenom + " " + c.Nom)
                .ToList();

            return Json(coachNames);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //pour créer les conversation entre chaque membre
        private int CreerConversations(int equipeId)
        {
            if (equipeId <= 0) return 0;

            var membres = _db.Utilisateurs
                .Where(u => u.EquipeId == equipeId)
                .Select(u => u.CustomId)
                .ToList();

            if (membres.Count < 2) return 0;

            int created = 0;

            for (int i = 0; i < membres.Count; i++)
            {
                for (int j = i + 1; j < membres.Count; j++)
                {
                    var id1 = membres[i];
                    var id2 = membres[j];

                    //existe deja ?
                    var existe = _msg.Conversations.Any(c =>
                        c.Participants.Count == 2 &&
                        c.Participants.Any(p => p.Id_utilisateur == id1) &&
                        c.Participants.Any(p => p.Id_utilisateur == id2));

                    if (existe) continue;

                    //cree la conversation et ajoute les participant
                    var conv = new Conversation
                    {
                        date_creation = DateTime.Now,
                        Titre = $"Conversation {id1}-{id2}"
                    };
                    _msg.Conversations.Add(conv);
                    _msg.SaveChanges(); 

                    _msg.ConversationUtilisateurs.Add(new ConversationUtilisateur
                    {
                        ConversationId = conv.Id,
                        Id_utilisateur = id1,
                        ajouter_a = DateTime.Now
                    });
                    _msg.ConversationUtilisateurs.Add(new ConversationUtilisateur
                    {
                        ConversationId = conv.Id,
                        Id_utilisateur = id2,
                        ajouter_a = DateTime.Now
                    });

                    _msg.SaveChanges();
                    created++;
                }
            }

            return created;
        }
    }

}