from __future__ import annotations

from datetime import datetime, timezone
from enum import Enum
from threading import Lock
from typing import Dict, List, Optional
from uuid import UUID, uuid4

from fastapi import FastAPI, HTTPException, Query, status
from pydantic import BaseModel, ConfigDict, Field

app = FastAPI(title="OrganizationService", version="1.0.0")


class OrganizationType(str, Enum):
    garage = "Garage"
    law_firm = "LawFirm"
    clinic = "Clinic"
    company = "Company"


class OrganizationStatus(str, Enum):
    active = "Active"
    suspended = "Suspended"


class CreateOrganizationRequest(BaseModel):
    model_config = ConfigDict(populate_by_name=True)

    name: str = Field(..., min_length=1)
    type: OrganizationType
    siret: Optional[str] = Field(default=None, min_length=14, max_length=14)
    owner_id: Optional[UUID] = Field(default=None, alias="ownerId")


class UpdateOrganizationRequest(BaseModel):
    model_config = ConfigDict(populate_by_name=True)

    name: str = Field(..., min_length=1)
    type: OrganizationType
    siret: Optional[str] = Field(default=None, min_length=14, max_length=14)


class OrganizationResponse(BaseModel):
    model_config = ConfigDict(populate_by_name=True)

    id: UUID
    name: str
    type: OrganizationType
    status: OrganizationStatus
    siret: Optional[str]
    owner_id: Optional[UUID] = Field(default=None, alias="ownerId")
    created_at: datetime = Field(alias="createdAt")
    updated_at: datetime = Field(alias="updatedAt")


class OrganizationStore:
    def __init__(self) -> None:
        self._items: Dict[UUID, OrganizationResponse] = {}
        self._lock = Lock()

    def create(self, payload: CreateOrganizationRequest) -> OrganizationResponse:
        now = datetime.now(timezone.utc)
        organization = OrganizationResponse(
            id=uuid4(),
            name=payload.name,
            type=payload.type,
            status=OrganizationStatus.active,
            siret=payload.siret,
            owner_id=payload.owner_id,
            created_at=now,
            updated_at=now,
        )
        with self._lock:
            self._items[organization.id] = organization
        return organization

    def get(self, organization_id: UUID) -> OrganizationResponse:
        with self._lock:
            organization = self._items.get(organization_id)
        if organization is None:
            raise KeyError
        return organization

    def list(self, owner_id: Optional[UUID]) -> List[OrganizationResponse]:
        with self._lock:
            organizations = list(self._items.values())
        if owner_id is None:
            return organizations
        return [item for item in organizations if item.owner_id == owner_id]

    def update(self, organization_id: UUID, payload: UpdateOrganizationRequest) -> OrganizationResponse:
        with self._lock:
            organization = self._items.get(organization_id)
            if organization is None:
                raise KeyError
            updated = organization.model_copy(
                update={
                    "name": payload.name,
                    "type": payload.type,
                    "siret": payload.siret,
                    "updated_at": datetime.now(timezone.utc),
                }
            )
            self._items[organization_id] = updated
        return updated

    def delete(self, organization_id: UUID) -> None:
        with self._lock:
            if organization_id not in self._items:
                raise KeyError
            del self._items[organization_id]


store = OrganizationStore()


@app.post("/api/organizations", response_model=OrganizationResponse, status_code=status.HTTP_201_CREATED)
async def create_organization(request: CreateOrganizationRequest) -> OrganizationResponse:
    return store.create(request)


@app.get("/api/organizations/{organization_id}", response_model=OrganizationResponse)
async def get_organization(organization_id: UUID) -> OrganizationResponse:
    try:
        return store.get(organization_id)
    except KeyError as exc:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Organization not found") from exc


@app.get("/api/organizations", response_model=List[OrganizationResponse])
async def list_organizations(owner_id: Optional[UUID] = Query(default=None, alias="ownerId")) -> List[OrganizationResponse]:
    return store.list(owner_id)


@app.put("/api/organizations/{organization_id}", response_model=OrganizationResponse)
async def update_organization(
    organization_id: UUID, request: UpdateOrganizationRequest
) -> OrganizationResponse:
    try:
        return store.update(organization_id, request)
    except KeyError as exc:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Organization not found") from exc


@app.delete("/api/organizations/{organization_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_organization(organization_id: UUID) -> None:
    try:
        store.delete(organization_id)
    except KeyError as exc:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Organization not found") from exc
