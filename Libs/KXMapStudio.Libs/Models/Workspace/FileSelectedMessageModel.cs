namespace KXMapStudio.Libs.Models.Workspace;

public sealed class FileSelectedMessageModel(string value) : ValueChangedMessage<string>(value);