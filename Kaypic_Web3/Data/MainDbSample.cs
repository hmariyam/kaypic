using Kaypic_Web3.Models;

namespace Kaypic_Web3.Data
{
    public static class MainDbSample
    {
        public static void Initialize(MainDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Equipes.Any() || context.Utilisateurs.Any())
                return;

            var equipe1 = new Equipe { Nom = "Basketball Saint-Laurent", Coach_Id = 0 };
            var equipe2 = new Equipe { Nom = "Empowering Her Sports", Coach_Id = 0 };
            var equipe3 = new Equipe { Nom = "Saint-Laurent Spartans", Coach_Id = 0 };
            context.Equipes.AddRange(equipe1, equipe2, equipe3);
            context.SaveChanges();

            var utilisateurs = new Utilisateur[]
            { // Roles:  0=Parent, 1=Coach, 2=Joueur
                new Utilisateur { Role = TypeUtilisateur.Coach, Nom = "Moreau", Prenom = "Olivier", Age = 30, Courriel = "omoreau@gmail.com", Mdp = "Pass123", EquipeId = equipe1.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Parent, Nom = "Bouchard", Prenom = "Sylvie", Age = 30, Courriel = "sbouchard92@gmail.com", Mdp = "Pass123", EquipeId = equipe1.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Chevalier", Prenom = "Simon", Age = 15, Courriel = "monchevalier@gmail.com", Mdp = "Pass123", EquipeId = equipe1.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Parent, Nom = "Dubois", Prenom = "Isabelle", Age = 30, Courriel = "isabelle.dubois@gmail.com", Mdp = "Pass123", EquipeId = equipe1.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Parent, Nom = "AlHarash", Prenom = "Leen", Age = 30, Courriel = "zimbabweobjets@gmail.com", Mdp = "Pass123", EquipeId = equipe1.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Beaulieu", Prenom = "Nathan", Age = 15, Courriel = "nathan.beaulieu15@gmail.com", Mdp = "Pass123", EquipeId = equipe1.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Gagné", Prenom = "Émile", Age = 14, Courriel = "emile.gagne14@gmail.com", Mdp = "Pass123", EquipeId = equipe1.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Bouchard", Prenom = "Alyssa", Age = 20, Courriel = "alyssa.bouchard16@gmail.com", Mdp = "Pass123", EquipeId = equipe1.Id, Telephone = "+14384070858" },

                new Utilisateur { Role = TypeUtilisateur.Coach, Nom = "Gauthier", Prenom = "Anaïs", Age = 30, Courriel = "gauthier.anais@gmail.com", Mdp = "Pass123", EquipeId = equipe2.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Parent, Nom = "Fortin", Prenom = "Amélie", Age = 30, Courriel = "ameliefortin@gmail.com", Mdp = "Pass123", EquipeId = equipe2.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Fortin", Prenom = "Léa", Age = 19, Courriel = "leaf2015@gmail.com", Mdp = "Pass123", EquipeId = equipe2.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Parent, Nom = "Lavoie", Prenom = "Nathalie", Age = 30, Courriel = "nathalie.lavoie@gmail.com", Mdp = "Pass123", EquipeId = equipe2.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Parent, Nom = "Caron", Prenom = "Pascal", Age = 30, Courriel = "pascal.caron@gmail.com", Mdp = "Pass123", EquipeId = equipe2.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Caron", Prenom = "Julien", Age = 17, Courriel = "julien.caron17@gmail.com", Mdp = "Pass123", EquipeId = equipe2.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Daigle", Prenom = "Maya", Age = 20, Courriel = "maya.daigle15@gmail.com", Mdp = "Pass123", EquipeId = equipe2.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Tessier", Prenom = "Liam", Age = 14, Courriel = "liam.tessier14@gmail.com", Mdp = "Pass123", EquipeId = equipe2.Id, Telephone = "+14384070858" },

                new Utilisateur { Role = TypeUtilisateur.Coach, Nom = "Morel", Prenom = "Alexis", Age = 30, Courriel = "alexis-morel-pro@gmail.com", Mdp = "Pass123", EquipeId = equipe3.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Parent, Nom = "Laroche", Prenom = "Marc", Age = 30, Courriel = "laroche-entreprise@gmail.com", Mdp = "Pass123", EquipeId = equipe3.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Laroche", Prenom = "Gabriel", Age = 16, Courriel = "gablaroche@gmail.com", Mdp = "Pass123", EquipeId = equipe3.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Parent, Nom = "Bergeron", Prenom = "Annie", Age = 30, Courriel = "annie.bergeron@gmail.com", Mdp = "Pass123", EquipeId = equipe3.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Parent, Nom = "Morin", Prenom = "Philippe", Age = 30, Courriel = "philippe.morin@gmail.com", Mdp = "Pass123", EquipeId = equipe3.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Morin", Prenom = "Alexandre", Age = 20, Courriel = "alexandre.morin16@gmail.com", Mdp = "Pass123", EquipeId = equipe3.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Lambert", Prenom = "Camille", Age = 15, Courriel = "camille.lambert15@gmail.com", Mdp = "Pass123", EquipeId = equipe3.Id, Telephone = "+14384070858" },
                new Utilisateur { Role = TypeUtilisateur.Joueur, Nom = "Poirier", Prenom = "Elliot", Age = 17, Courriel = "elliot.poirier17@gmail.com", Mdp = "Pass123", EquipeId = equipe3.Id, Telephone = "+14384070858" }
            };

            context.Utilisateurs.AddRange(utilisateurs);
            context.SaveChanges();

            foreach(var utilisateur in utilisateurs)
            {
                utilisateur.Email = utilisateur.Courriel;
                utilisateur.PasswordHash = utilisateur.Mdp;
            }

            //update CoachId//
            equipe1.Coach_Id = utilisateurs.First(u => u.Nom == "Moreau" && u.Prenom == "Olivier").CustomId;
            equipe2.Coach_Id = utilisateurs.First(u => u.Nom == "Gauthier" && u.Prenom == "Anaïs").CustomId;
            equipe3.Coach_Id = utilisateurs.First(u => u.Nom == "Morel" && u.Prenom == "Alexis").CustomId;
            context.SaveChanges();

            var annonces = new Annonce[]
            {
                new Annonce {
                    Titre = "Rappel d’inscriptions - Saison d’été",
                    Description = "Dernier rappel pour inscrire votre enfant à la saison d’été prochaine...",
                    priorite = Priorite.Important,
                    Pinned = true,
                    Equipeid = equipe1.Id
                },
                new Annonce {
                    Titre = "Recherche covoiturage",
                    Description = "Est-ce que quelqu’un peut amener Léa dimanche ?",
                    priorite = Priorite.Urgent,
                    Pinned = false,
                    Equipeid = equipe2.Id
                },
                new Annonce {
                    Titre = "Résultats du dernier match",
                    Description = "Victoire des Spartans contre Rosemont, 8 à 7 grâce à Gabriel Laroche.",
                    priorite = Priorite.Régulier,
                    Pinned = false,
                    Equipeid = equipe3.Id
                }
            };
            context.Annonces.AddRange(annonces);
            context.SaveChanges();


            var calendriers = new Calendrier[]
            {
                new Calendrier {
                    Titre = "Pratique hebdomadaire",
                    Description = "Entraînement hebdo",
                    Date = new DateTime(2025, 10, 19),
                    Heure = new TimeSpan(11),
                    Lieu = "École Lauren Hill Jr",
                    type = Models.Type.Entrainement,
                    Equipeid = equipe1.Id,
                    Utilisateurid = utilisateurs.First(u => u.Nom == "Moreau" && u.Prenom == "Olivier").CustomId
                },
                new Calendrier {
                    Titre = "Premier match de la saison - Soccer AA (12-14 ans)",
                    Description = "Match contre Longueuil au Parc Maisonneuve",
                    Date = new DateTime(2026, 6, 6),
                    Heure = new TimeSpan(19),
                    Lieu = "Parc Maisonneuve, Montréal",
                    type = Models.Type.Match,
                    Equipeid = equipe2.Id,
                    Utilisateurid = utilisateurs.First(u => u.Nom == "Gauthier" && u.Prenom == "Anaïs").CustomId
                },
                new Calendrier {
                    Titre = "Message aux coachs - Prochaine saison",
                    Description = "Réunion pour règlements et planification de la prochaine saison.",
                    Date = new DateTime(2025, 8, 9),
                    Heure = new TimeSpan(9),
                    Lieu = "Complexe sportif Saint-Laurent",
                    type = Models.Type.Réunion,
                    Equipeid = equipe3.Id,
                    Utilisateurid = utilisateurs.First(u => u.Nom == "Morel" && u.Prenom == "Alexis").CustomId
                },
                new Calendrier {
                    Titre = "Rendez-vous parents",
                    Description = "Réunion avec le coach pour discuter des progrès de votre enfant",
                    Date = new DateTime(2025, 11, 25),
                    Heure = new TimeSpan(18, 0, 0),
                    Lieu = "Salle communautaire Saint-Laurent",
                    type = Models.Type.Réunion,
                    Equipeid = equipe1.Id,
                    Utilisateurid = utilisateurs.First(u => u.Nom == "AlHarash" && u.Prenom == "Leen").CustomId
                }
            };
            context.Calendriers.AddRange(calendriers);
            context.SaveChanges();
        }
    }
}