# KXMapStudio Architecture Documentation

This directory contains Architecture Decision Records (ADRs) and architectural documentation for the KXMapStudio solution.

## Solution Structure

KXMapStudio is a three-layer WPF application following clean architecture principles:

```
┌─────────────────────────────────────────────────────────────┐
│  KXMapStudio.Application                                    │
│  (Entry Point, DI Composition, Configuration)               │
│  - App.xaml / App.xaml.cs                                   │
│  - Composition root                                         │
│  - Resource dictionaries                                    │
└────────────────────────────┬────────────────────────────────┘
                             │ depends on
                             ▼
┌─────────────────────────────────────────────────────────────┐
│  KXMapStudio.Libs (.NET 10 - Windows)                       │
│  (Presentation Layer - MVVM, Views, UI Services)            │
│  - ViewModels (ObservableObject, INotifyPropertyChanged)   │
│  - Views (XAML, UserControls)                               │
│  - UI Services (Dialogs, Notifications, Hotkeys)           │
│  - Material Design 3 themes                                 │
└────────────────────────────┬────────────────────────────────┘
                             │ depends on
                             ▼
┌─────────────────────────────────────────────────────────────┐
│  KXMapStudio.Core (.NET 10)                                 │
│  (Domain Layer - UI-Agnostic Business Logic)                │
│  - Models (Domain entities, DTOs)                           │
│  - Services (Business rules, domain logic)                  │
│  - Repositories (Data persistence abstractions)             │
│  - Facades (Simplified interfaces for complex operations)   │
└─────────────────────────────────────────────────────────────┘
```

## Architectural Principles

### 1. **Strict Layer Separation**
- **Core** MUST NOT reference Libs or Application
- **Core** MUST NOT contain any WPF types (`Dispatcher`, `UIElement`, `DependencyObject`, etc.)
- **Libs** MAY reference Core
- **Application** MAY reference both Libs and Core

### 2. **Dependency Injection First**
- All services and ViewModels are registered via constructor injection
- No `new` instantiation of ViewModels or services in production code
- DI containers:
  - Core: `ServiceCollectionExtensions.AddCoreDependencies()`
  - Libs: `ServiceCollectionExtension.AddLibsDependencies()`

### 3. **Interface-Driven Design**
- All services expose interfaces (e.g., `IXxxService`, `IXxxRepository`)
- Interfaces are defined in `Abstractions/` folders
- Implementations depend on abstractions, not concrete types

### 4. **MVVM Pattern (Libs Layer)**
- ViewModels use `CommunityToolkit.Mvvm` source generators
  - `[ObservableProperty]` for observable state
  - `[RelayCommand]` for commands
- ViewModels orchestrate, services implement
- Code-behind MUST NOT contain business logic

### 5. **Async-First**
- All I/O operations MUST be async
- Methods accept `CancellationToken` where appropriate
- Use `ConfigureAwait(false)` in Core library code

### 6. **Comprehensive Documentation**
- All public classes, interfaces, and methods have XML documentation
- Documentation follows Microsoft style:
  - `<summary>` for purpose
  - `<param>` for parameters
  - `<returns>` for return values
  - `<remarks>` for additional context
  - `<exception>` for exceptions

## Key Patterns

### Service Layer Pattern
Business logic is extracted into dedicated services:
- **Core Services**: Business rules, domain transformations
  - Examples: `IMumbleMarkerService`, `IFileValidationService`
- **Libs Services**: UI coordination, state management
  - Examples: `IGridRowManipulationService`, `IStateManagementService<T>`

### Repository Pattern
Data persistence is abstracted through repositories:
- `IJsonRepository` - Generic JSON serialization
- `IFileStorageRepository` - File deletion operations
- Located in `Core/Repositories/`

### Facade Pattern
Complex operations are simplified through facades:
- `IFileFacade` - Simplified file operations
- Located in `Core/Facades/`

## Naming Conventions

| Type | Convention | Example |
|------|------------|---------|
| Interface | `IXxx` | `IGridEditorViewModel` |
| Service | `XxxService` | `MumbleMarkerService` |
| Repository | `XxxRepository` | `JsonRepository` |
| ViewModel | `XxxViewModel` | `GridEditorViewModel` |
| Model | `XxxModel` | `GridRowModel` |
| Data Transfer Object | `XxxData` or `XxxDto` | `GridRowData` |

## Namespace Organization

```
KXMapStudio.Core
├── Abstractions/
│   ├── Facades/
│   ├── Http/
│   ├── Repositories/
│   └── Services/
│       ├── Markers/
│       ├── Mumble/
│       ├── Serializations/
│       └── Validation/
├── Models/
├── Repositories/
├── Services/
│   ├── Http/
│   ├── Markers/
│   ├── Mumble/
│   ├── Serializations/
│   └── Validation/
└── Types/

KXMapStudio.Libs
├── Abstractions/
│   ├── Services/
│   │   ├── Dialogs/
│   │   ├── GridEditor/
│   │   ├── Hotkeys/
│   │   ├── StateManagement/
│   │   └── WorkshopExplorer/
│   └── ViewModels/
├── Models/
├── Services/
├── ViewModels/
└── Views/

KXMapStudio.Application
├── Views/
│   └── Windows/
└── (Composition root files)
```

## Technology Stack

- **.NET 10** (Latest LTS)
- **WPF** (Windows Presentation Foundation)
- **CommunityToolkit.Mvvm** (MVVM helpers, source generators)
- **MaterialDesignThemes** (Material Design 3 UI components)
- **Microsoft.Extensions.DependencyInjection** (DI container)
- **Microsoft.Extensions.Logging** (Logging infrastructure)
- **Gw2Sharp** (Guild Wars 2 API client)

## Architecture Decision Records

- [ADR-001: Service Layer Refactoring](./ADR-001-service-layer-refactoring.md)

## Build Requirements

- **.NET 10 SDK** or later
- **Windows** environment (for WPF support)
- **Visual Studio 2022** (recommended) or Rider

### Building

```bash
# Build entire solution
dotnet build KXMapStudio.slnx

# Build Core only (platform-agnostic)
cd Libs/KXMapStudio.Core
dotnet build
```

## Testing Strategy

Currently, the solution does not have automated tests but follows patterns that enable testability:
- Services use interfaces (easy to mock)
- ViewModels depend on abstractions (injectable test doubles)
- Business logic is separated from UI (unit testable)

Future testing considerations:
- Unit tests for Core services
- Integration tests for Libs services
- UI tests for Views (optional)

## Code Quality Standards

### Complexity Limits
- Classes: Target < 300 lines
- Methods: Target < 40 lines
- Cyclomatic complexity: Keep low through service extraction

### Style Guide
- Follow .NET naming conventions
  - `PascalCase` for public members
  - `camelCase` for private fields (underscore prefix allowed)
- Use file-scoped namespaces
- Prefer `record` for immutable data carriers
- Use modern C# features (pattern matching, primary constructors, global usings)

### Comments Policy
- XML documentation REQUIRED for all public APIs
- Inline comments ONLY for "why", not "what"
- No obsolete TODO/FIXME comments
- Design decisions should be documented (see GridEditorViewModel remarks)

## Future Considerations

### Potential Improvements
- Extract additional services as ViewModels grow
- Consider implementing automated tests
- Evaluate async patterns for better cancellation support
- Monitor ViewModel sizes for extraction opportunities

### Maintenance Guidelines
- Keep architectural boundaries strict (Core ↔ Libs ↔ Application)
- Prefer extraction over inline logic in ViewModels
- Maintain comprehensive XML documentation
- Follow established patterns for consistency

---

**Last Updated**: 2026-01-24  
**Maintained By**: Development Team
