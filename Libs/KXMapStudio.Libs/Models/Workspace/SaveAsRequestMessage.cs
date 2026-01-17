namespace KXMapStudio.Libs.Models.Workspace;

/// <summary>
///     Message requesting a save as operation.
/// </summary>
public sealed record SaveAsRequestMessage(string FilePath);