# Prompt: Add the service to manage register of supervised entities

**Date**: 2025-10-05 06:55:08  
**Branch**: Frontend-updates

## User Request
Add the service to manage register of supervised entities

## Context
The user requested to create an Angular service for managing the register of supervised entities (Polish: "Kartoteka Podmiotów"). This service will connect the frontend with the existing backend API endpoints for supervised entity management.

## Analysis
1. **Backend Investigation**: The backend already has a complete implementation:
   - `EntityManagementService` in `UknfCommunicationPlatform.Infrastructure/Services/`
   - `EntitiesController` in `UknfCommunicationPlatform.Api/Controllers/v1/`
   - DTOs: `EntityResponse`, `EntityListItemResponse`, `CreateEntityRequest`, `UpdateEntityRequest`
   - REST API endpoints: GET (list & details), POST (create), PUT (update), DELETE (soft delete)

2. **Frontend Gap**: No entity service existed in the frontend services directory (`frontend/uknf-project/src/app/services/`)

3. **Requirements Analysis**:
   - From `.requirements/UI_SCREENS_SUMMARY.md`: The system needs entity management capabilities
   - From `ADMIN_MODULE_REQUIREMENTS.md`: The EntitiesController is marked as HIGH priority
   - From `.requirements/DETAILS_UKNF_Prompt2Code2.md`: The system must provide entity data updater service for registration data
   - Backend already provides: listing, filtering, pagination, CRUD operations, user management, CSV import

## Implementation

### Created Files
1. **`frontend/uknf-project/src/app/services/supervised-entity.service.ts`** - New Angular service

### Service Features
The new `SupervisedEntityService` provides:

1. **Entity Listing**:
   - `getEntities()` - Paginated list with filtering (searchTerm, entityType, isActive)
   - Support for sorting by field and order
   - Returns `EntityListResponse` with pagination metadata

2. **Entity Details**:
   - `getEntityById()` - Full entity information including statistics

3. **CRUD Operations**:
   - `createEntity()` - Create new supervised entity with full validation
   - `updateEntity()` - Update existing entity (including soft-delete via isActive flag)
   - `deleteEntity()` - Soft delete (sets isActive to false)

4. **User Management**:
   - `getEntityUsers()` - List all users assigned to an entity

5. **Bulk Import**:
   - `importEntitiesFromCsv()` - Upload CSV file for bulk entity import

### TypeScript Interfaces Defined
All interfaces match backend DTOs exactly:
- `SupervisedEntity` - Full entity details (matches `EntityResponse`)
- `SupervisedEntityListItem` - List view (matches `EntityListItemResponse`)
- `CreateEntityRequest` - Entity creation payload
- `UpdateEntityRequest` - Entity update payload
- `EntityUser` - User assigned to entity
- `EntityFilters` - Search filters
- `EntityPagination` - Pagination metadata
- `EntityListResponse` - Paginated list response

### API Endpoints Used
- `GET /api/v1/entities` - List with filters
- `GET /api/v1/entities/{id}` - Get details
- `POST /api/v1/entities` - Create entity
- `PUT /api/v1/entities/{id}` - Update entity
- `DELETE /api/v1/entities/{id}` - Delete entity
- `GET /api/v1/entities/{id}/users` - Get entity users
- `POST /api/v1/entities/import` - CSV import

### Key Design Decisions
1. **Naming**: Used "SupervisedEntity" prefix to match backend naming convention
2. **Injectable Service**: Used Angular 16+ inject() function pattern (same as other services)
3. **HTTP Client**: Follows HttpClient pattern with HttpParams for query parameters
4. **API URL**: Uses localhost:5000 (consistent with other services)
5. **Type Safety**: Full TypeScript interfaces with proper null handling
6. **Documentation**: Comprehensive JSDoc comments on all methods and interfaces

## Benefits
- ✅ Complete type-safe interface for supervised entity management
- ✅ Consistent with existing service patterns in the codebase
- ✅ Ready for integration with future entity management UI components
- ✅ Supports all backend features including CSV import
- ✅ Proper error handling via Observable patterns
- ✅ Comprehensive filtering and pagination support

## Next Steps
This service can now be used to build:
1. Entity list/table component with search and filters
2. Entity details view component
3. Entity create/edit forms
4. Entity-user management interface
5. CSV import dialog/component

## Compliance
- ✅ Follows Angular service patterns from existing codebase
- ✅ Matches backend API contracts exactly
- ✅ Proper TypeScript typing throughout
- ✅ Includes comprehensive documentation
- ✅ Supports requirements from `.requirements/DETAILS_UKNF_Prompt2Code2.md`
