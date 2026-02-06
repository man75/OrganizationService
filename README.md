# OrganizationService — Microservice (Garage / Cabinet / Société)

Microservice **Organisation (tenant)** réutilisable pour iGarage et d’autres domaines
(cabinet d’avocat, clinique, société de services, etc.).

- **Architecture** : API REST FastAPI + Pydantic
- **Stack** : Python, FastAPI, Uvicorn

---

## 🎯 Objectif

Gérer la **structure Organisation** :
- créer une organisation (garage, cabinet, clinique…)
- modifier les informations (nom, type, SIRET)
- (optionnel) gérer les membres et leurs rôles

> ⚠️ L’authentification n’est pas encore branchée.
> Le `UserId` peut être mocké temporairement (ou passé via query param `ownerId`) en attendant le JWT.

---

## 🌐 Endpoints (MVP)

Organizations
- `POST /api/organizations` → créer une organisation
- `GET /api/organizations/{id}` → récupérer une organisation
- `PUT /api/organizations/{id}` → mettre à jour une organisation
- `DELETE /api/organizations/{id}` → supprimer une organisation

Listing (temporaire sans auth)
- `GET /api/organizations?ownerId=...`

🔐 Quand l’auth sera branchée :
- on supprime `ownerId` des DTO
- on lit le `UserId` depuis les claims JWT

---

## 🧾 DTO principaux

### CreateOrganizationRequest
```json
{
  "name": "Garage Dupont",
  "type": "Garage",
  "siret": "12345678901234",
  "ownerId": "f5f8c58c-2a12-49a0-b0cc-2a4a8d5a8ec8"
}
```

### UpdateOrganizationRequest
```json
{
  "name": "Garage Dupont & Fils",
  "type": "Garage",
  "siret": "12345678901234"
}
```

---

## ⚙️ Lancer le service

```bash
python -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
uvicorn src.organization_service_fastapi.main:app --reload
```

Swagger :
- http://localhost:8000/docs

---

## 🧠 Notes importantes

### Persistance
Cette version utilise un **stockage en mémoire** (dict Python) pour rester légère.
Pour la production, prévoir un stockage PostgreSQL (ex: SQLAlchemy + Alembic).

### SIRET
`Siret` doit être une chaîne de 14 caractères.

### Sécurité
Une Organization n’a pas d’OwnerUserId côté client.
La sécurité se fera via OrganizationMember + Role quand l’auth sera branchée.

Le `UserId` vient du backend, jamais du client.
