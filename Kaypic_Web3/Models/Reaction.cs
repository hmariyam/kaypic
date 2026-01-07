using Microsoft.AspNetCore.Identity;

namespace Kaypic_Web3.Models
{
    public class Reaction
    {
        public int MessageId { get; set; }
        public virtual HistoriqueMessages Message { get; set; }
        public int UserId { get; set; }
        public string Emoji { get; set; }
    }
}