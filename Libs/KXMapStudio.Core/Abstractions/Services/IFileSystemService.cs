namespace KXMapStudio.Core.Abstractions.Services;

public interface IFileSystemService
{
	/// <summary>
	///     Creates a new file with the specified type at the given path.
	/// </summary>
	Task<string> CreateFileAsync(string directoryPath, string fileName, FileType fileType,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Creates a new directory at the specified path.
	/// </summary>
	Task<string> CreateDirectoryAsync(string parentPath, string directoryName,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Deletes a file at the specified path.
	/// </summary>
	Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Deletes a directory recursively at the specified path.
	/// </summary>
	Task DeleteDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Creates a new XML file inside an archive (.zip or .taco).
	/// </summary>
	Task CreateFileInArchiveAsync(string archivePath, string fileName, CancellationToken cancellationToken = default);

	/// <summary>
	///     Validates if a file name is valid and doesn't conflict with existing files.
	/// </summary>
	bool ValidateFileName(string directoryPath, string fileName, out string? errorMessage);

	/// <summary>
	///     Validates if a directory name is valid and doesn't conflict with existing directories.
	/// </summary>
	bool ValidateDirectoryName(string parentPath, string directoryName, out string? errorMessage);

	/// <summary>
	///     Checks if a path is the root Data folder and cannot be deleted.
	/// </summary>
	bool IsRootDataFolder(string path);
}