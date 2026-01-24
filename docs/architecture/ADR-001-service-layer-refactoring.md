# Architecture Decision Record: Service Layer Refactoring

## Status
Accepted - January 2026

## Context
The KXMapStudio solution had excellent separation of concerns, but some opportunities existed to further improve architectural clarity:

1. **Business Logic in Libs Layer**: Mumble marker coordinate extraction was implemented in the GridEditorViewModel (Libs layer), which is a UI concern. This business rule should be in Core.

2. **Direct File System Calls**: WorkshopExplorerViewModel used `File.Exists()` directly, bypassing the abstraction pattern used elsewhere in Core.

3. **ViewModel Complexity**: GridEditorViewModel contained row manipulation logic that could be extracted into a dedicated service for better testability and reusability.

## Decision
We decided to extract and organize business logic into dedicated services:

### 1. **IMumbleMarkerService** (Core Layer)
**Location**: `Libs/KXMapStudio.Core/Services/Markers/`

**Rationale**: 
- Converting Mumble player position to marker coordinates is a **business rule**, not a UI concern
- The logic for generating marker names (`"Marker {counter}"`) is domain knowledge
- Validation of Mumble state suitability belongs in Core

**API**:
```csharp
string GenerateMarkerName(int counter);
bool CanCreateMarkerFromMumbleState(MumbleStateModel mumbleState);
(double X, double Y, double Z) ExtractCoordinatesFromMumble(MumbleStateModel mumbleState);
```

**Impact**: 
- Libs layer ViewModels now delegate to Core for marker creation business logic
- Improved testability (Core services can be unit tested without UI dependencies)
- Clear separation: Core = "what to create", Libs = "when and how to present"

### 2. **IFileValidationService** (Core Layer)
**Location**: `Libs/KXMapStudio.Core/Services/Validation/`

**Rationale**:
- File existence validation is a business concern (e.g., "can we delete this file?")
- Direct `File.Exists()` calls bypass the architectural abstraction pattern
- Consistency with existing patterns (IFileFacade, IFileStorageRepository)

**API**:
```csharp
bool FileExists(string filePath);
bool DirectoryExists(string directoryPath);
bool IsJsonFile(string filePath);
```

**Impact**:
- WorkshopExplorerViewModel no longer uses static `File` class directly
- Maintains architectural boundary between UI and file system
- Enables future enhancements (e.g., virtual file systems, testing stubs)

### 3. **IGridRowManipulationService** (Libs Layer)
**Location**: `Libs/KXMapStudio.Libs/Services/GridEditor/RowManipulation/`

**Rationale**:
- Row manipulation (move, reindex, validation) was scattered across GridEditorViewModel
- These operations are UI coordination concerns (managing observable collections)
- Extracting to a service improves testability and reduces ViewModel complexity

**API**:
```csharp
void ReindexIds(IList<GridEditorRowViewModel> rows);
bool CanMoveUp(IList<GridEditorRowViewModel> rows, GridEditorRowViewModel? row);
bool CanMoveDown(IList<GridEditorRowViewModel> rows, GridEditorRowViewModel? row);
int GetRowIndex(IList<GridEditorRowViewModel> rows, GridEditorRowViewModel? row);
```

**Impact**:
- GridEditorViewModel focuses on orchestration, not implementation details
- Service is reusable across ViewModels if needed
- Clearer method signatures in ViewModel (delegates to service)

## Consequences

### Positive
- ✅ **Clearer architectural boundaries**: Business logic in Core, UI coordination in Libs
- ✅ **Improved testability**: Services can be tested independently
- ✅ **Better maintainability**: Logic is organized by concern, not by ViewModel
- ✅ **Consistency**: All services follow the same interface-driven pattern
- ✅ **Comprehensive documentation**: All new services have XML docs

### Neutral
- Line counts: GridEditorViewModel went from 804 → 814 lines (added DI parameters)
  - However, the *complexity* is reduced (logic delegated to services)
  - ViewModels are now thinner orchestrators
  
- File count increased: Core (39 → 46), Libs (49 → 53)
  - Trade-off: More files, but each has a single, clear responsibility

### Negative
- Slightly increased DI surface area (more constructor parameters)
  - Mitigated by: Constructor parameters are well-documented and logically grouped

## Notes
- **No breaking changes**: All interfaces were added, not modified
- **Backward compatible**: Existing functionality is preserved
- **DI Registration**: All services are registered as singletons in their respective DI containers
  - Core: `ServiceCollectionExtensions.AddCoreDependencies()`
  - Libs: `ServiceCollectionExtension.AddLibsDependencies()`

## Related Patterns
- **Service Layer Pattern**: Business logic extracted into dedicated services
- **Repository Pattern**: Already established (IFileStorageRepository, IJsonRepository)
- **Facade Pattern**: Already established (IFileFacade)
- **MVVM Pattern**: ViewModels remain orchestrators, delegating to services

## Compliance
This refactoring aligns with the existing architectural patterns:
- Core: UI-agnostic, business rules only
- Libs: MVVM presentation layer, UI orchestration
- Application: Composition root, DI configuration

---

**Author**: GitHub Copilot  
**Date**: 2026-01-24  
**Approved by**: Refactoring Task Requirements
