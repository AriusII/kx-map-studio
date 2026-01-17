namespace KXMapStudio.Libs.Models.Workspace;

/// <summary>
///     Message sent when the editor state changes (undo/redo/save availability).
/// </summary>
public sealed record EditorStateChangedMessage(bool CanUndo, bool CanRedo, bool CanSave);