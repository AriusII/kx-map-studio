global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Globalization;
global using System.IO;
global using System.IO.Compression;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Timers;

// ─────────────────────────────────────────────────────────────────────
// WPF Framework
// ─────────────────────────────────────────────────────────────────────
global using System.Windows;
global using System.Windows.Controls;
global using System.Windows.Data;
global using System.Windows.Input;
global using System.Windows.Interop;
global using System.Windows.Threading;
global using Microsoft.Win32;

// ─────────────────────────────────────────────────────────────────────
// Third-party Libraries (CommunityToolkit.Mvvm)
// ─────────────────────────────────────────────────────────────────────
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;

// ─────────────────────────────────────────────────────────────────────
// KXMapStudio.Core (Domain Layer) - Abstractions
// ─────────────────────────────────────────────────────────────────────
global using KXMapStudio.Core.Abstractions.Repositories;
global using KXMapStudio.Core.Abstractions.Services;
global using KXMapStudio.Core.Abstractions.Services.Mumble;
global using KXMapStudio.Core.Abstractions.Services.Serializations;

// ─────────────────────────────────────────────────────────────────────
// KXMapStudio.Core (Domain Layer) - Models & Types
// ─────────────────────────────────────────────────────────────────────
global using KXMapStudio.Core.Models;
global using KXMapStudio.Core.Models.Serializations.Json;
global using KXMapStudio.Core.Types.Enums;
global using KXMapStudio.Core.Types.Structs;

// ─────────────────────────────────────────────────────────────────────
// KXMapStudio.Libs (Presentation Layer) - Abstractions
// ─────────────────────────────────────────────────────────────────────
global using KXMapStudio.Libs.Abstractions.Services.Dialogs;
global using KXMapStudio.Libs.Abstractions.Services.GridEditor;
global using KXMapStudio.Libs.Abstractions.Services.Hotkeys;
global using KXMapStudio.Libs.Abstractions.Services.StateManagement;
global using KXMapStudio.Libs.Abstractions.Services.WorkshopExplorer;
global using KXMapStudio.Libs.Abstractions.ViewModels;
global using KXMapStudio.Libs.Abstractions.ViewModels.BottomSide.StatusBar;
global using KXMapStudio.Libs.Abstractions.ViewModels.LeftSide;
global using KXMapStudio.Libs.Abstractions.ViewModels.LeftSide.WorkshopExplorer;
global using KXMapStudio.Libs.Abstractions.ViewModels.RightSide.GridEditor;

// ─────────────────────────────────────────────────────────────────────
// KXMapStudio.Libs (Presentation Layer) - Models
// ─────────────────────────────────────────────────────────────────────
global using KXMapStudio.Libs.Models.Editor;
global using KXMapStudio.Libs.Models.Grid;
global using KXMapStudio.Libs.Models.LeftSide.WorkshopExplorer;

// ─────────────────────────────────────────────────────────────────────
// KXMapStudio.Libs (Presentation Layer) - Services
// ─────────────────────────────────────────────────────────────────────
global using KXMapStudio.Libs.Services.Dialogs;
global using KXMapStudio.Libs.Services.GridEditor;
global using KXMapStudio.Libs.Services.Hotkeys;
global using KXMapStudio.Libs.Services.StateManagement;
global using KXMapStudio.Libs.Services.WorkshopExplorer;

// ─────────────────────────────────────────────────────────────────────
// KXMapStudio.Libs (Presentation Layer) - ViewModels
// ─────────────────────────────────────────────────────────────────────
global using KXMapStudio.Libs.ViewModels;
global using KXMapStudio.Libs.ViewModels.BottomSide.StatusBar;
global using KXMapStudio.Libs.ViewModels.LeftSide;
global using KXMapStudio.Libs.ViewModels.LeftSide.WorkshopExplorer;
global using KXMapStudio.Libs.ViewModels.RightSide.GridEditor;

// ─────────────────────────────────────────────────────────────────────
// Microsoft Extensions (Dependency Injection & Logging)
// ─────────────────────────────────────────────────────────────────────
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

// ─────────────────────────────────────────────────────────────────────
// Type Aliases (Disambiguation)
// ─────────────────────────────────────────────────────────────────────
global using Timer = System.Timers.Timer;
global using Constants = KXMapStudio.Core.Types.Structs.Constants;