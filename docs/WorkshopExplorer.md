# Workshop Explorer - Architecture & Implementation

## Overview

The Workshop Explorer is a file browser component in KXMapStudio that displays the contents of the `Data/` folder (relative to the executable). It functions like a simplified version of Visual Studio Code's file explorer, allowing users to browse directories and files (filtered to XML and JSON only).

## Architecture

### Component Structure

The Workshop Explorer follows a clean layered architecture with 2 main services and 1 ViewModel.

### Key Components

#### 1. **WorkshopExplorerViewModel**
- **Responsibility**: UI state management and user interaction
- **Features**:
  - Manages `ObservableCollection<WorkspaceExplorerNodeModel>` for tree display
  - Monitors filesystem changes via `FileSystemWatcher`
  - Debounced refresh (250ms) to avoid UI flicker
  - Preserves expanded/collapsed state across refreshes
  - Restores selection after refresh
- **Threading**: Uses WPF dispatcher for UI updates from filesystem watcher

#### 2. **WorkshopExplorerService**
- **Responsibility**: Filesystem scanning and orchestration
- **Features**:
  - Builds tree from `DataFolder` (exe path + `/Data`)
  - Filters files to XML and JSON only (via `WorkshopExplorerConstants`)
  - Recursively scans directories, skipping hidden/system folders
  - Sorts: directories first, then files (case-insensitive alphabetical)
  - Path validation for filesystem change relevance
- **Performance**: Runs on background threads via `Task.Run()`

#### 3. **WorkshopExplorerNodeService**
- **Responsibility**: Tree manipulation utilities
- **Features**:
  - Generic `TraverseTree()` for tree operations
  - `FindByPath()` for node lookup
  - `ExpandParents()` for automatic expansion to target node
  - `MapScanNodeToUiNode()` converts immutable scan results to observable UI models
  - Static path utilities (`NormalizeFullPath`, `PathEquals`, `PathStartsWith`)

#### 4. **WorkshopExplorerConstants**
- **Responsibility**: Centralized configuration
- **Contains**:
  - `AllowedFileExtensions`: `[".xml", ".json"]`
  - `PathComparer`: `StringComparer.OrdinalIgnoreCase`

## Refresh Strategy

### Non-blocking Refresh
The Workshop Explorer uses a "last request wins" refresh strategy:

1. **Debouncing**: Multiple rapid FS changes trigger a single refresh (250ms delay)
2. **Cancellation**: New refresh cancels previous in-progress refresh
3. **State Preservation**:
   - Captures expanded folder paths before scan
   - Restores expansion state after scan
   - Re-selects previously selected node (if still exists)
4. **Background Execution**: Filesystem scan runs on thread pool via `Task.Run()`
5. **UI Thread Safety**: Results are applied on WPF dispatcher

### FileSystemWatcher Configuration
```csharp
IncludeSubdirectories = true
NotifyFilter = FileName | DirectoryName | LastWrite
```

Monitored events:
- `Created` - New file/folder
- `Deleted` - Removed file/folder
- `Changed` - File modification
- `Renamed` - Rename operation

## Filtering Rules

### File Extensions
Only files with these extensions are shown:
- `.xml`
- `.json`

### Directory Exclusions
Directories are skipped if:
- `FileAttributes.Hidden` is set
- `FileAttributes.System` is set

## Code Optimization History

### Optimization Round 1 (Current PR)

**Changes Made**:
1. Created `WorkshopExplorerConstants` to centralize configuration
2. Enhanced `WorkshopExplorerNodeService` with generic tree utilities
3. Merged `WorkshopExplorerScanner` into `WorkshopExplorerService`
4. Consolidated path normalization and comparison methods
5. Simplified ViewModel tree traversal using `TraverseTree()`

**Impact**:
- **Net code reduction**: ~50 lines of improved, more maintainable code
- **Files deleted**: 2 (Scanner service + interface)
- **Abstractions reduced**: From 3 services to 2

**Benefits**:
- Single source of truth for constants
- Easier to extend (e.g., add `.yaml` support)
- Better testability (fewer dependencies)
- Clearer service boundaries
