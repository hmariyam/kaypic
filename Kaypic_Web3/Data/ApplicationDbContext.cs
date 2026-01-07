using Kaypic_Web3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kaypic_Web3.Data
{
    // Inherit from IdentityDbContext to integrate ASP.NET Identity
    public class MainDbContext : IdentityDbContext<Utilisateur>
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Annonce> Annonces { get; set; }
        public DbSet<Calendrier> Calendriers { get; set; }
        public DbSet<Equipe> Equipes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Utilisateur>()
                .HasAlternateKey(u => u.CustomId);

            modelBuilder.Entity<Utilisateur>()
                .HasOne(u => u.Equipe)
                .WithMany()
                .HasForeignKey(u => u.EquipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Calendrier>()
                .HasOne(c => c.Equipe)
                .WithMany()
                .HasForeignKey(c => c.Equipeid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Calendrier>()
                .HasOne(c => c.utilisateur)
                .WithMany()
                .HasForeignKey(c => c.Utilisateurid)
                .HasPrincipalKey(u => u.CustomId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


    public class MessagingDbContext : DbContext
    {
        public MessagingDbContext(DbContextOptions<MessagingDbContext> options) : base(options) { }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationFichiers> ConversationFichiers { get; set; }
        public DbSet<ConversationUtilisateur> ConversationUtilisateurs { get; set; }
        public DbSet<HistoriqueMessages> HistoriqueMessages { get; set; }
        public DbSet<HistoriqueAppels> HistoriqueAppels { get; set; }
        public DbSet<Reaction> Reactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reaction>()
                .HasKey(r => new { r.MessageId, r.UserId });

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Message)
                .WithMany(m => m.Reactions)
                .HasForeignKey(r => r.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. HistoriqueMessages Relationship
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Message)
                .WithMany(m => m.Reactions)
                .HasForeignKey(r => r.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}