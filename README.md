# Kaypic_Web3
Une platforme de communication inspirée de la plateforme existante https://circle.kaypic.com/

## Authors
- Leen Al Harash
- Mariyam Hanfaoui
- Émile Larochelle-Langevin

## Diagramme du BD
![Diagramme](https://git.dti.crosemont.quebec/lal/kaypic_web3/-/raw/main/Fichiers/Diagramme.png?ref_type=heads)

## Migrations
L'application utilise Entity Framework Core pour gérer les migrations.

* **Supprimer toutes les migrations et la base de données :**

```bash
dotnet ef database drop --context MainDbContext --force
dotnet ef database drop --context MessagingDbContext --force

dotnet ef migrations remove --context MainDbContext 
dotnet ef migrations remove --context MessagingDbContext
```

* **Recréer la migration :**

```bash
dotnet ef migrations add InitialCreate --context MainDbContext
dotnet ef migrations add InitMessaging --context MessagingDbContext
```

* **Mettre à jour la base de données :**

```bash
dotnet ef database update --context MainDbContext
dotnet ef database update --context MessagingDbContext
```

## Tâches accomplies par chaque membre d'équipe

| Membre | Tâches accomplies |
|--------|------------------|
| 2234270,<br>Leen Al Harash | - Création du projet et de la page d'accueil<br> - Réalisation des maquettes<br> - Création de la base de données avec l'API Kaypic<br> - Suppression des annonces<br> - Création des paramètres pour les comptes<br> - Changement du mot de passe<br> - Authentification selon les rôles<br> - Envoi de fichiers dans le chat<br> - Affichage de l'heure et de la date dans les messages<br> - Ajout d'emojis sur les messages<br> - Questions/Réponses pour le compte parent<br> - Authentification via Google<br> - CSS complet du site web<br> - Documentation ReadMe et powerpoint |
| 2240026,<br>Mariyam Hanfaoui | - Création des diagrammes de la base de données<br> - Création de la base de données avec l'API Kaypic<br> - Adaptation de la base de données <br> - Création, modification et suppression des calendriers<br> - Mise en place de la MFA pour les comptes "Twilio"<br> - Rendre le filtrage des événements dynamique<br> - Création de groupe<br> - Messagerie en groupe<br> - Ajout d’un filtre pour l’âge des joueurs<br> - Ajout d'emojis sur les messages<br> - Suppression de compte et envoi de rappel par SMS<br> - Rappel d'événements par SMS |
| 2252359,<br>Émile Larochelle-Langevin | - Création de la base de données avec l'API Kaypic<br> - Chiffrement des mots de passe dans la base de données<br> - Création et modification des annonces<br> - Rendre le filtrage des annonces dynamique<br> - Messagerie entre des utilisateurs 1v1<br> - Messagerie en groupe<br> - Appels entre utilisateurs<br> - Appels en groupe<br> - Ajout de l'API Swagger |
