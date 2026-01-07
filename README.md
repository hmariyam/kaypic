# Kaypic_Web3
Une platforme de communication inspirée de la plateforme existante https://circle.kaypic.com/

## Authors
- Leen Al Harash
- Mariyam Hanfaoui
- Émile Larochelle-Langevin

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
