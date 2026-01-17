# OrganizationService — Microservice (Garage / Cabinet / Société)

Microservice **Organisation (tenant)** réutilisable pour iGarage et d’autres domaines
(cabinet d’avocat, clinique, société de services, etc.).

- **Architecture** : Microservice + Clean Architecture + DDD pragmatique + CQRS light (MediatR)
- **Stack** : .NET 9, EF Core, PostgreSQL, Swagger

---

## 🎯 Objectif

Gérer la **structure Organisation** :
- créer une organisation (garage, cabinet, clinique…)
- modifier les informations (nom, type, SIRET)
- (optionnel) gérer les membres et leurs rôles

> ⚠️ L’authentification n’est pas encore branchée.  
> Le `UserId` peut être mocké temporairement (ou passé via header) en attendant le JWT.

---

## 🧱 Architecture (Clean Architecture)

src/
OrganizationService.Api # Adaptateur HTTP (Controllers, DTO)
OrganizationService.Application # Use cases (Commands / Queries + Handlers)
OrganizationService.Domain # Modèle métier (Aggregates, règles)
OrganizationService.Infrastructure # EF Core, DbContext, Repositories

pgsql
Copier le code

### Dépendances
- Api → Application → Domain
- Infrastructure → Application → Domain  
👉 Le **Domain ne dépend de rien**.

---

## 🧠 Patterns utilisés

- **Mediator Pattern** → MediatR
- **CQRS light** → séparation Command / Query (même base)
- **DDD tactique** → Aggregate Root, Entities, invariants métier
- **Repository Pattern** → abstraction de la persistance

---

## 📊 Diagramme de classes (Domain)

```mermaid
classDiagram
direction LR

class Organization {
  -List~OrganizationMember~ _members
  +Guid Id
  +string Name
  +OrganizationType Type
  +OrganizationStatus Status
  +string? Siret
  +DateTime CreatedAt
  +DateTime UpdatedAt
  +IReadOnlyCollection~OrganizationMember~ Members
  +Rename(name)
  +Update(name, type, siret)
  +InviteMember(actorUserId, userId, role)
  +ChangeMemberRole(actorUserId, userId, newRole)
  +DisableMember(actorUserId, userId)
}

class OrganizationMember {
  +Guid Id
  +Guid OrganizationId
  +Guid UserId
  +MemberRole Role
  +MemberStatus Status
  +DateTime CreatedAt
  +ChangeRole(role)
  +Disable()
  <<Entity>>
}

class OrganizationType {
  <<enum>>
  Garage
  LawFirm
  Clinic
  Company
}

class OrganizationStatus {
  <<enum>>
  Active
  Suspended
}

class MemberRole {
  <<enum>>
  OrgAdmin
  Staff
}

class MemberStatus {
  <<enum>>
  Invited
  Active
  Disabled
}

Organization "1" o-- "0..*" OrganizationMember : members
Organization --> OrganizationType
Organization --> OrganizationStatus
OrganizationMember --> MemberRole
OrganizationMember --> MemberStatus
Si la gestion des membres n’est pas encore implémentée, tu peux ignorer
OrganizationMember et les méthodes associées.

🔁 Diagramme de séquence — Create Organization
mermaid
Copier le code
sequenceDiagram
    participant Client
    participant API as OrganizationsController
    participant Med as MediatR
    participant H as CreateOrganizationHandler
    participant D as Organization (Domain)
    participant R as OrganizationRepository
    participant DB as PostgreSQL

    Client->>API: POST /api/organizations\nCreateOrganizationRequest
    API->>Med: Send(CreateOrganizationCommand)
    Med->>H: Resolve handler + Handle(command)
    H->>D: new Organization(...)
    H->>R: Add(Organization)
    H->>R: SaveChangesAsync()
    R->>DB: INSERT organizations
    DB-->>R: OK
    R-->>H: id
    H-->>Med: id
    Med-->>API: id
    API-->>Client: 201 Created + Location
🌐 Endpoints (MVP)
Organizations
POST /api/organizations → créer une organisation

GET /api/organizations/{id} → récupérer une organisation

PUT /api/organizations/{id} → mettre à jour une organisation

DELETE /api/organizations/{id} → supprimer une organisation

Listing (temporaire sans auth)
GET /api/organizations?ownerId=...

🔐 Quand l’auth sera branchée :

on supprime ownerId des DTO

on lit le UserId depuis les claims JWT

🧾 DTO principaux
CreateOrganizationRequest
json
Copier le code
{
  "name": "Garage Dupont",
  "type": "Garage",
  "siret": "12345678901234"
}
UpdateOrganizationRequest
json
Copier le code
{
  "name": "Garage Dupont & Fils",
  "type": "Garage",
  "siret": "12345678901234"
}
⚙️ Configuration
appsettings.json (API)
json
Copier le code
{
  "ConnectionStrings": {
    "OrganizationDb": "Host=localhost;Port=5432;Database=organization_db;Username=postgres;Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
▶️ Lancer le service
powershell
Copier le code
dotnet restore
dotnet build
dotnet run --project .\src\OrganizationService.Api\OrganizationService.Api.csproj
Swagger :

bash
Copier le code
https://localhost:<port>/swagger
🗄️ Migrations EF Core
Dans Package Manager Console (Visual Studio) :

powershell
Copier le code
Add-Migration InitOrganizationDb `
  -Project OrganizationService.Infrastructure `
  -StartupProject OrganizationService.Api `
  -OutputDir Persistence\Migrations

Update-Database `
  -Project OrganizationService.Infrastructure `
  -StartupProject OrganizationService.Api
En cas d’erreur “ConnectionString not initialized”, garder
OrganizationDbContextFactory dans Infrastructure (design-time).

🧠 Notes importantes
SIRET
Organization doit exposer :

csharp
Copier le code
public string? Siret { get; private set; }
Mapping EF requis :

csharp
Copier le code
b.Property(x => x.Siret)
 .HasColumnName("siret")
 .HasMaxLength(14);
Éviter EF.Property(o, "Siret") sauf shadow property explicite.

Sécurité
Une Organization n’a pas d’OwnerUserId

La sécurité se fait via OrganizationMember + Role

Le UserId vient du backend, jamais du client
