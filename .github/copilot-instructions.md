# GitHub Copilot Instructions (Policy)

## Purpose

This document defines **non-negotiable engineering policies** for contributions generated or modified by GitHub Copilot in the **KXMapStudio** solution.

The goal is to keep the codebase:

- **Layered and maintainable** (clear boundaries and ownership)
- **Testable and DI-first** (interface-driven and mock-friendly)
- **Performant and robust** (streaming I/O, minimal allocations, defensive failure policies)
- **WPF/MVVM-correct** (no UI leakage into domain code)

> **Normative language**
>
> - **MUST / MUST NOT / REQUIRED**: mandatory.
> - **SHOULD / SHOULD NOT**: expected by default; exceptions require clear justification.
> - **MAY**: optional.

---

## Solution architecture (hard boundaries)

KXMapStudio is a **3-layer** solution:

1. `Libs/KXMapStudio.Core`
   - Domain model + business-critical workflows.
   - Serialization (XML/JSON), archive reading (ZIP/TACO), I/O repositories, HTTP integrations, and core services.
   - **UI-agnostic**.

2. `Libs/KXMapStudio.Libs`
   - Presentation logic library for WPF using MVVM.
   - ViewModels, UI services, UI models, and Material Design integration helpers.
   - Depends on `.Core` abstractions.

3. `Srcs/KXMapStudio.Application`
   - WPF application entry point.
   - Composition root: config, DI container, theme wiring, main window, and shell.

### Dependency direction (MUST)

- `.Core` **MUST NOT** reference `.Libs` or `.Application`.
- `.Libs` **MAY** reference `.Core`.
- `.Application` **MAY** reference `.Libs` and `.Core`.

### “Where does this code go?” decision rule

- If the code needs **WPF types** (e.g., `Dispatcher`, `DependencyObject`, `UIElement`, `ResourceDictionary`, `RoutedEventArgs`), **it MUST NOT be in `.Core`**.
- If the code contains **business/data rules**, parsing, validation, or file-format correctness, it **MUST be in `.Core`**.
- If the code is primarily **observable UI state + commands**, it **MUST be in `.Libs`** (and exposed through interfaces).
- If the code wires DI, config, resources, or creates windows, it **MUST be in `.Application`**.

---

## Layer policies

### `.Core` (domain + infrastructure, performance-first)

`.Core` is a pure class library targeting `net10.0`.

#### `.Core` MUST

- Remain **UI-agnostic**.
- Prefer **streaming APIs** for I/O (e.g., `Stream`, `CopyToAsync`, `XDocument.LoadAsync`).
- Expose capabilities via **small interfaces** under `Abstractions/`.
- Keep file system/serialization concerns behind **repositories**:
  - `IFileStorageRepository`, `IXmlDataRepository`, `IJsonDataRepository`, `IArchiveDataRepository`.
- Implement workflows in **services** under `Services/`.
- Implement format conversion in **mappers** under `Mappers/` (e.g., `TacoMapper`).
- Use DI registration via `KXMapStudio.Core.Extensions.ServiceCollectionExtensions.AddCoreDependencies()`.
- Make I/O and long-running operations **async** and accept `CancellationToken`.
- Validate public inputs (null, whitespace, out-of-range) using guard clauses.

#### `.Core` MUST NOT

- Reference WPF / UI frameworks (MaterialDesign, CommunityToolkit.Mvvm WPF types, `Dispatcher`, etc.).
- Perform synchronous blocking I/O on public paths.
- Use global mutable state to store runtime data (except for well-scoped constants/options).

#### Failure policy (REQUIRED)

- Parsing-heavy operations **MAY** be tolerant and return `null` for malformed inputs when that is the established behavior.
  - Example: XML/TACO deserialization may return `null` rather than throwing.
- Infrastructure boundary violations **SHOULD** fail fast with exceptions (e.g., invalid enum values).
- HTTP integrations **MUST** be defensive at application startup boundaries.
  - Example: `GithubHttpClient.CheckCurrentVersion()` returns `false` on failures to avoid blocking the app.

#### Core workflows (reference symbols)

- TACO/ZIP/XML reading:
  - Primary entry point: `IFileReaderService.ReadTacoAsync(string filePath, FileType fileType)`.
  - `FileType.Xml` → `IXmlService.LoadFromFileAsync`.
  - `FileType.Zip` / `FileType.Taco` → `IArchiveService.LoadMarkerPackAsync` → `IXmlService.LoadFromStreamAsync`.

- GW2 JSON assets:
  - `IFileReaderService.ReadMapsJsonAsync()` → `IJsonService.LoadGuildWarsMapsAsync()`.
  - `IFileReaderService.ReadContinentsJsonAsync()` → `IJsonService.LoadGuildWarsContinentFloorAsync()`.

---

### `.Libs` (WPF MVVM presentation library)

`.Libs` is a WPF class library targeting `net10.0-windows` and is the **presentation orchestration layer**.

#### `.Libs` MUST

- Implement MVVM using **CommunityToolkit.Mvvm**.
- Expose **interfaces for all ViewModels and services** under `Abstractions/`.
- Keep ViewModels **thin**:
  - orchestrate UI behavior,
  - delegate domain work to `.Core` services (prefer abstractions).
- Use **async-first** patterns and cancellation for long operations.
- Implement `IDisposable` for:
  - file watchers (`FileSystemWatcher`),
  - timers,
  - event subscriptions,
  - `CancellationTokenSource`.
- Register dependencies via `KXMapStudio.Libs.Extensions.ServiceCollectionExtension.AddLibsDependencies()`.

#### `.Libs` MUST NOT

- Implement business rules already owned by `.Core`.
- Perform file parsing/serialization directly when `.Core` provides repositories/services.
- Create or manage WPF windows as the primary composition root (that belongs in `.Application`).

#### MVVM rules (REQUIRED)

- ViewModels **MUST** derive from `ObservableObject` (or an approved base) and use source generators:
  - `[ObservableProperty]` for observable state.
  - `[RelayCommand]` (or explicit `IRelayCommand`) for commands.
- ViewModels **MUST NOT** reference WPF visual types (`Control`, `UIElement`, `Window`) in their public API.
- Views (`Views/`) **SHOULD** be XAML-first with minimal code-behind.
  - Code-behind **MUST NOT** contain business logic.

#### Event-driven communication (REQUIRED)

- Prefer decoupled communication patterns consistent with the existing style:
  - e.g., `WorkshopExplorerViewModel.FileSelected` event, consumed by `LeftSidePanelViewModel`.
- Subscriptions **MUST** be paired with unsubscription in `Dispose()`.

#### Performance rules for UI collections (REQUIRED)

- Do not replace `ObservableCollection<T>` instances frequently.
- Prefer `Clear()` + `Add()` batching to update UI collections.

---

### `.Application` (composition root)

`.Application` owns:

- App startup (`App.xaml`, `App.xaml.cs`)
- DI container creation
- Resource dictionaries and Material Design theme wiring
- Main window creation and DataContext assignment

#### `.Application` MUST

- Be the only layer that:
  - creates the root `IServiceProvider`,
  - instantiates Windows/shell,
  - loads global themes/resources.
- Register dependencies by calling:
  - `services.AddCoreDependencies();`
  - `services.AddLibsDependencies();`

#### `.Application` SHOULD

- Keep “composition root” code minimal and declarative (wire up; do not implement domain logic).

---

## Dependency Injection (DI) policy

### Registration modules (REQUIRED)

- `.Core` registrations **MUST** live behind `AddCoreDependencies()`.
- `.Libs` registrations **MUST** live behind `AddLibsDependencies()`.
- `.Application` **MUST NOT** register individual internals that are owned by `.Core` or `.Libs` unless required for composition.

### Lifetimes (REQUIRED)

- Follow existing conventions:
  - ViewModels: `Singleton` by default (shared app state).
  - Services: `Singleton` when stateless or when owning watchers/timers.
- If multi-window independent state is introduced:
  - Use `Scoped` and create a scope per window.
  - Document and enforce the scope boundaries.

### Abstraction-first usage (REQUIRED)

- Code **MUST** depend on interfaces (e.g., `IFileReaderService`, `IWorkshopExplorerService`).
- Concrete implementations are internal concerns of modules.

---

## Async, cancellation, and threading

### Async-first (REQUIRED)

- Public methods performing I/O **MUST** be `async` and accept `CancellationToken`.
- Do not block on tasks (`.Result`, `.Wait()`) on UI paths.

### Cancellation semantics (REQUIRED)

- If an operation can be superseded (e.g., file preview loads), the newest request **MUST** cancel the previous one.
  - Example: `FilePreviewViewModel` maintains a `CancellationTokenSource` and cancels before starting a new load.

### ConfigureAwait

- In `.Core`, library code **SHOULD** use `ConfigureAwait(false)` where appropriate (non-UI code), unless it breaks established patterns.
- In `.Libs`, avoid manually forcing context switches; rely on WPF synchronization context unless you are explicitly on background threads.

---

## I/O and serialization policy

### General rules (REQUIRED)

- Prefer **stream-based** APIs.
- Avoid reading entire files into strings unless the format requires it.
- Use `System.Text.Json` for JSON.
- Use `System.Xml.Linq` (`XDocument`) + `XmlSerializer` for XML as established in `.Core`.

### JSON options (REQUIRED)

When writing JSON in `.Core`, preserve the established deterministic behavior:

- `WriteIndented = true`
- ignore null values when writing (`JsonIgnoreCondition.WhenWritingNull`)
- relaxed escaping when needed

### ZIP/TACO policy (REQUIRED)

- `.taco` is treated as a ZIP archive.
- Never extract to temporary files unless a workflow explicitly requires it.
- Archive reading **MUST** operate via `ZipArchive` streams.

---

## Documentation and style (Microsoft-style)

### C# style (REQUIRED)

- Follow .NET naming guidelines:
  - `PascalCase` for public types and members.
  - `camelCase` for locals and private fields (underscore prefix allowed by existing code style).
- Use file-scoped namespaces where consistent.
- Prefer `record` for immutable data carriers (e.g., `FilePreviewResult`).

### XML documentation comments

- Public interfaces and public-facing services **SHOULD** have XML doc comments describing:
  - purpose,
  - parameters,
  - return values,
  - failure behavior,
  - cancellation behavior.

### Logging

- `.Core` hosted services **SHOULD** use `ILogger` (as already done by `KXMapStudio.Core.Host.Initialization`).
- Do not log excessively in hot paths.

---

## Contribution playbooks (how Copilot should implement changes)

### Add a new file format in `.Core` (REQUIRED steps)

1. Add DTOs under `Libs/KXMapStudio.Core/Models/` (appropriate subfolder).
2. Add mapping logic under `Libs/KXMapStudio.Core/Mappers/`.
3. Add serialization/workflow service under `Libs/KXMapStudio.Core/Services/Serializations/`.
4. If persistence is required, add a repository under `Libs/KXMapStudio.Core/Repositories/` and an interface under `Abstractions/Repositories/`.
5. Register new services/repositories in `AddCoreDependencies()`.
6. Provide async overloads that accept `CancellationToken`.

### Add a new external integration (HTTP) in `.Core` (REQUIRED steps)

1. Define interface under `Abstractions/Http/` (example: `IGuildWarsHttpClient`, `IGithubHttpClient`).
2. Implement under `Services/Http/`.
3. Use `HttpClient` (via `IHttpClientFactory` when applicable) and keep methods async.
4. Define failure policy explicitly (throw vs return default).
5. Register the client in `AddCoreDependencies()`.

### Add a new ViewModel in `.Libs` (REQUIRED steps)

1. Create an interface under `Libs/KXMapStudio.Libs/Abstractions/ViewModels/`.
2. Implement in `Libs/KXMapStudio.Libs/ViewModels/`.
3. Use `ObservableObject` and source generators (`[ObservableProperty]`, `[RelayCommand]`) where appropriate.
4. Keep business logic out; delegate to `.Core` services.
5. Implement `IDisposable` if the ViewModel subscribes to events, owns timers/watchers, or uses `CancellationTokenSource`.
6. Register in `AddLibsDependencies()`.

### Add a new UI service in `.Libs` (REQUIRED steps)

1. Create an interface under `Abstractions/Services/`.
2. Implement under `Services/`.
3. The service may map domain models → UI models (e.g., `FileSystemEntryNodeModel` → `WorkspaceExplorerNodeModel`).
4. Keep it testable and DI-friendly.

---

## Prohibited patterns (MUST NOT)

- `.Core` referencing WPF or UI frameworks.
- ViewModels performing synchronous I/O or blocking waits.
- Code-behind implementing business rules.
- Creating ViewModels with `new` in production code (use DI).
- Long-lived event subscriptions without unsubscription.

---

## Verification checklist (Copilot output MUST satisfy)

When generating changes, Copilot MUST ensure:

- Code is placed in the correct layer and folder.
- New public artifacts have interfaces in `Abstractions/` where applicable.
- Async methods accept `CancellationToken`.
- Resources are disposed correctly (`IDisposable` implemented where needed).
- DI registration is updated via `AddCoreDependencies()` / `AddLibsDependencies()`.
- Changes preserve performance characteristics (streaming I/O, minimal allocations).
