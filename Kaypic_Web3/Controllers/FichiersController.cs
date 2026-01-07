using Kaypic_Web3.Data;
using Kaypic_Web3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kaypic_Web3.Controllers
{
    public class FichiersController : Controller
    {
        private readonly MessagingDbContext _context;

        public FichiersController(MessagingDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, int conversationId)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            var fileName = Path.GetFileName(file.FileName);

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileRecord = new ConversationFichiers
            {
                ConversationId = conversationId,
                CreerParIdUtilisateur = HttpContext.Session.GetInt32("UtilisateurID") ?? 0,
                FileName = fileName,
                FileUrl = "/uploads/" + fileName,
                TypeFichier = TypeFichier.Fichier
            };

            _context.ConversationFichiers.Add(fileRecord);
            await _context.SaveChangesAsync();

            return Ok(new { fileRecord.Id, fileRecord.FileName, fileRecord.FileUrl });
        }


        [HttpGet("conversation/{id}")]
        public async Task<IActionResult> GetConversation(int id)
        {
            var fichiers = await _context.ConversationFichiers
                .Where(f => f.ConversationId == id)
                .OrderBy(f => f.DateCreation)
                .ToListAsync();

            var conversationVM = new ConversationVM
            {
                ConversationId = id,
                CurrentUserId = HttpContext.Session.GetInt32("UtilisateurID") ?? 0,
                Fichiers = fichiers
            };

            return Ok(conversationVM);
        }



        public IActionResult Index()
        {
            return View();
        }
    }
}