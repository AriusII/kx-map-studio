# KXMapStudio.Core

> **KXMapStudio’s performance-first domain layer** for **.NET 10 / C# 14**.
>
> This project contains the *heavy* and *business-critical* code: file workflows, serialization (XML/JSON), archive
> reading (ZIP/TACO), domain models/mappers, HTTP integrations, and core services designed to be **fast, robust,
testable,
and UI-agnostic**.

---

## Table of Contents

- [What this project is](#what-this-project-is)
- [Architecture in the solution (Core → Libs → Application)](#architecture-in-the-solution-core--libs--application)
- [Project boundaries (what belongs here)](#project-boundaries-what-belongs-here)
- [Tech stack & requirements](#tech-stack--requirements)
- [Folder map (source layout)](#folder-map-source-layout)
- [Dependency Injection (DI) & hosting](#dependency-injection-di--hosting)
    - [`AddCoreDependencies()`](#addcoredependencies)
    - [Hosted initialization (`Initialization`)](#hosted-initialization-initialization)
- [Public API surface (key abstractions)](#public-api-surface-key-abstractions)
- [Core workflows](#core-workflows)
    - [Read TACO marker packs from `.xml` / `.taco` / `.zip`](#read-taco-marker-packs-from-xml--taco--zip)
    - [Read GW2 JSON assets (Maps / Continents)](#read-gw2-json-assets-maps--continents)
- [Serialization & file formats](#serialization--file-formats)
    - [XML (TACO overlay)](#xml-taco-overlay)
    - [JSON (KX + GW2 payloads)](#json-kx--gw2-payloads)
    - [ZIP / TACO archives](#zip--taco-archives)
- [Repositories (I/O boundaries)](#repositories-io-boundaries)
- [HTTP integrations](#http-integrations)
- [State management primitives](#state-management-primitives)
- [Domain models & mapping](#domain-models--mapping)
- [Performance, robustness, and engineering rules](#performance-robustness-and-engineering-rules)
- [Extending Core safely](#extending-core-safely)
- [Building](#building)

---

## What this project is

`KXMapStudio.Core` is a **pure Class Library** designed as a **domain + business logic** package.

It exists to:

- own the **domain model** (strongly typed models used by the rest of the solution)
- implement **structured file workflows** (XML, JSON)
- read domain-specific **archives** (`.zip` / `.taco`) as `ZipArchive`
- expose **services/repositories** through clean interfaces
- provide **integration primitives** (HTTP clients, state history)

This library is intentionally built to be:

- **High-performance**: streaming-first I/O, minimal allocations, predictable cost
- **Robust**: validated boundaries, tolerant parsing where it makes sense
- **Composable**: dependency-injection friendly, minimal global state
- **UI-agnostic**: no WPF types, no view models, no UI concerns

---

## Architecture in the solution (Core → Libs → Application)

The full solution is decomposed into 3 layers:

1. **`Libs/KXMapStudio.Core`** *(this project)*
    - Domain model, heavy workflows, serialization, repositories, parsing, core services.

2. **`Libs/KXMapStudio.Libs`** *(depends on `.Core`)*
    - WPF-oriented logic: MVVM state machines, ViewModels, UI composition helpers.

3. **`Srcs/KXMapStudio.Application`** *(depends on `.Libs`)*
    - WPF entrypoint and composition root.
    - Uses UI frameworks such as **MaterialDesign** and **CommunityToolkit.Mvvm**.

**Rule of thumb:** if code needs WPF / UI concepts, it does **not** belong in `.Core`.

---

## Project boundaries (what belongs here)

✅ **Belongs in `.Core`:**

- domain models and mappers
- pure services (parsing, conversion, workflows)
- repositories that abstract persistence (file system, serialization)
- HTTP clients for external APIs (GW2, GitHub)
- thread-safe non-UI state utilities (undo/redo stacks, caching primitives)

❌ **Does not belong in `.Core`:**

- ViewModels, commands, UI state
- WPF resources, styles, control logic
- direct references to MaterialDesign / CommunityToolkit.Mvvm
- anything that requires `Dispatcher` or UI threading

---

## Tech stack & requirements

- **Target Framework:** `.NET 10` (`net10.0`)
- **Language:** C# 14 (solution-wide)
- **Key packages:**
    - `Microsoft.Extensions.Hosting` / `Microsoft.Extensions.Http` for DI + HttpClient
    - `Gw2Sharp` for GW2 Mumble + API integration

Core uses modern .NET building blocks:

- `System.Text.Json` for JSON
- `System.Xml.Linq` + `XmlSerializer` for XML
- `System.IO.Compression` for `.zip` / `.taco`
- `async` + `CancellationToken` across I/O boundaries

---

## Folder map (source layout)

The `.Core` project is organized by responsibility:

- `Abstractions/`
    - contracts (`I*`) for services, repositories and HTTP clients
- `Services/`
    - domain workflows & orchestrators (e.g. file reading)
    - `Services/Serializations/` for XML/JSON/Archive operations
    - `Services/Http/` for external API wrappers
    - `Services/StateManagement/` for non-UI state utilities
- `Repositories/`
    - persistence boundaries (file system, JSON/XML read/write, archive access)
- `Models/`
    - domain models and DTOs (JSON models, XML DTOs, value objects)
- `Mappers/`
    - mapping logic (DTO ↔ domain)
- `Types/`
    - small enums/struct-like constants (`FileType`, `FileExtension`, etc.)
- `Host/`
    - hosted startup tasks (filesystem bootstrap)
- `Extensions/`
    - DI composition entry point (`AddCoreDependencies()`)

---

## Dependency Injection (DI) & hosting

`.Core` is designed to be consumed through **Microsoft.Extensions.DependencyInjection**.

### `AddCoreDependencies()`

`KXMapStudio.Core.Extensions.ServiceCollectionExtensions` exposes a single entry point:

- registers repositories (filesystem + serialization)
- registers domain services
- registers HTTP clients
- registers a hosted initialization service

This is intentionally a **composition module**: upper projects call it, but `.Core` owns its internal wiring.

Example usage from a consuming layer:

```csharp
using KXMapStudio.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCoreDependencies();
```

> In the full application, `KXMapStudio.Application` is the composition root; `.Libs` and `.Core` only provide modules.

### Hosted initialization (`Initialization`)

`KXMapStudio.Core.Host.Initialization` is registered as an `IHostedService` and is responsible for **bootstrapping the
runtime data folder**:

- computes the folder relative to the app base directory
- ensures `./Data` exists (see `Constants.Settings.DataFolder`)
- logs success/failure via `ILogger`

This keeps “first-run filesystem setup” **out of UI code** and makes startup deterministic.

---

## Public API surface (key abstractions)

The primary contracts live under `Abstractions/`.

### Serialization services

- `IXmlService`
    - load/save TACO marker packs from file or stream
    - parse `XDocument` into a domain marker pack
- `IJsonService`
    - load/save KX JSON
    - load GW2 payloads (`Maps`, `Continents`)
- `IArchiveService`
    - enumerate archive entries
    - load a marker pack from `.taco`/`.zip` entry (stream-based)

### Core workflow

- `IFileReaderService`
    - “smart entry point” to read TACO packs and JSON assets depending on type

### Infrastructure abstractions

- `IFileStorageRepository`
    - minimal file storage boundary (exists/load/save)
- `IXmlDataRepository`, `IJsonDataRepository`, `IArchiveDataRepository`
    - serialization-specific storage boundaries

### Integrations

- `IGuildWarsHttpClient` (GW2 API payloads)
- `IGithubHttpClient` (version-check via GitHub releases API)

### State management

- `IStateHistoryService<TState>`
    - generic undo/redo history for immutable state snapshots

---

## Core workflows

### Read TACO marker packs from `.xml` / `.taco` / `.zip`

The main entry point is `IFileReaderService.ReadTacoAsync(filePath, fileType)`.

Behavior:

- `FileType.Xml` → uses `IXmlService.LoadFromFileAsync`
- `FileType.Zip` / `FileType.Taco` → uses `IArchiveService.LoadMarkerPackAsync`
- everything else → throws `ArgumentOutOfRangeException`

This gives upper layers a **single** workflow and keeps format-specific complexity inside `.Core`.

### Read GW2 JSON assets (Maps / Continents)

Also via `IFileReaderService`:

- `ReadMapsJsonAsync()` → `IJsonService.LoadGuildWarsMapsAsync()`
- `ReadContinentsJsonAsync()` → `IJsonService.LoadGuildWarsContinentFloorAsync()`

---

## Serialization & file formats

### XML (TACO overlay)

Implementation: `KXMapStudio.Core.Services.Serializations.XmlService`

- uses an `XmlSerializer` targeting `TacoOverlayDataDto`
- loads XML via `IXmlDataRepository` into `XDocument`
- maps DTO → domain via `TacoMapper.MapToDomain()`
- maps domain → DTO via `TacoMapper.MapToDto()`

Key engineering characteristics:

- **stream-friendly APIs**: load/save to `Stream` to support archive scenarios
- **tolerant parsing**: deserialization errors are caught and result in `null`
- **round-trippable structure**: domain models can be serialized back to XML

### JSON (KX + GW2 payloads)

Repositories: `JsonDataRepository` + service `JsonService`

- `System.Text.Json` with deterministic options:
    - `WriteIndented = true` (human-readable diffs)
    - ignore nulls when writing (`WhenWritingNull`)
    - relaxed escaping to preserve content where needed

> Design note: JSON is treated as **data**, not behavior. Parsing/serialization belongs in repositories/services;
> business rules belong in higher-level services.

### ZIP / TACO archives

A `.taco` file is treated as a **ZIP archive** with domain meaning.

- `ArchiveDataRepository.LoadAsync()` opens a `ZipArchive` in **Read** mode
- `ArchiveService.LoadMarkerPackAsync()`:
    - enumerates entries
    - selects a specific entry if specified, otherwise selects the first `.xml`
    - opens the entry as a stream and delegates to `IXmlService.LoadFromStreamAsync()`

This stays **stream-based** end-to-end and avoids extracting temporary files.

---

## Repositories (I/O boundaries)

Repositories encapsulate the “impure” parts (filesystem + serialization I/O) behind small interfaces.

- `FileStorageRepository`
    - writes with `FileStream` + `CopyToAsync`
    - loads with an async-enabled read `FileStream`
    - creates directories as needed

- `XmlDataRepository`
    - loads XML from file/stream into `XDocument` (`XDocument.LoadAsync`)
    - writes XML with `XmlWriter` configured for:
        - UTF-8 without BOM
        - indentation
        - async I/O

- `JsonDataRepository`
    - serializes into a `MemoryStream`, then saves via `IFileStorageRepository`

- `ArchiveDataRepository`
    - opens ZIP archives and enumerates entries

> Architectural intent: upper layers should never directly call `File.ReadAllText()` / `XDocument.Load()` / `ZipArchive`
> for domain data—those details live here.

---

## HTTP integrations

### GitHub version check

`GithubHttpClient.CheckCurrentVersion()`:

- fetches latest release metadata from `Constants.Settings.GitHubApiUrl`
- compares release tag (e.g. `v1.2.3`) to the executing assembly version
- returns `true` if a newer version exists

The method is intentionally defensive: failures return `false` to avoid blocking the app.

### Guild Wars 2 API client

`GuildWarsHttpClient`:

- fetches raw JSON via `HttpClient.GetStringAsync`
- deserializes into `ContinentFloorModel` / `IReadOnlyList<MapModel>`

> Note: this client assumes payload correctness (uses null-forgiving `!`). Callers should wrap it at the application
> boundary if they need user-facing error reporting.

---

## State management primitives

`StateHistoryService<TState>` implements `IStateHistoryService<TState>` as a generic **undo/redo** buffer:

- stack-based history
- bounded memory via `maxHistorySize` (default: `20`)
- designed for **immutable state snapshots** (common in MVVM reducers)

Although MVVM lives in `.Libs`, the *primitive* history mechanism stays in `.Core` because it’s UI-agnostic and
reusable.

---

## Domain models & mapping

The `Models/` + `Mappers/` layout follows a clear separation:

- **DTOs**: represent external formats (XML attributes, JSON naming, loose types)
- **Domain models**: represent internal invariants and strongly typed state
- **Mappers**: explicit conversion functions between formats

Example: `TacoMapper`

- normalizes culture-dependent numbers via `CultureInfo.InvariantCulture`
- validates mandatory fields and discards invalid POIs/trails (returns `null` per-item)
- preserves nested category hierarchies

---

## Performance, robustness, and engineering rules

Core is written under strict constraints:

1. **Fast paths first**
    - prefer streaming APIs (`Stream`, `XDocument.LoadAsync`, `CopyToAsync`)
    - avoid loading whole archives into memory unless required by a workflow

2. **Explicit boundaries & validation**
    - public methods validate parameters (`ThrowIfNullOrWhiteSpace`, `ThrowIfNull`)

3. **Failure policy is intentional**
    - parsing-heavy operations may return `null` when input is malformed (tolerant mode)
    - infrastructural operations may throw when invariants break (fail fast)

4. **Async + cancellation**
    - I/O methods surface `CancellationToken`
    - do not block caller threads (especially UI threads in upper layers)

5. **Immutability-friendly**
    - prefer records/immutable models for state snapshots
    - use `StateHistoryService<T>` with immutable states to guarantee correctness

6. **No UI dependencies**
    - `.Core` stays clean so it can be unit-tested and reused (console tools, future services, etc.)

---

## Extending Core safely

When adding features, keep the architecture consistent:

- New file format?
    - add models under `Models/`
    - add a mapper under `Mappers/`
    - add a service under `Services/Serializations/`
    - add a repository if persistence/IO is needed

- New external integration?
    - add an `Abstractions/Http/I*HttpClient`
    - implement it under `Services/Http/`
    - register it in `AddCoreDependencies()`

- New domain workflow?
    - keep orchestration in a *service*
    - keep persistence details in *repositories*
    - keep format conversion in *mappers*

**Compatibility rule:** keep public abstractions stable (interfaces), and version behaviors intentionally.

---

## Building

This project targets `net10.0` and is part of the `KXMapStudio` solution.

- Build the solution via Rider/Visual Studio, or with `dotnet build`.

> If you add new dependencies, prefer well-established packages and keep allocations/I/O patterns under control.
