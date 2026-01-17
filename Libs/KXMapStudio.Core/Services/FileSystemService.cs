namespace KXMapStudio.Core.Services;

public sealed class FileSystemService(string dataFolderPath) : IFileSystemService
{
	private readonly string _dataFolderPath = dataFolderPath;

	public async Task<string> CreateFileAsync(string directoryPath, string fileName, FileType fileType,
		CancellationToken cancellationToken = default)
	{
		if (!ValidateFileName(directoryPath, fileName, out var errorMessage))
			throw new InvalidOperationException(errorMessage);

		var extension = fileType switch
		{
			FileType.Xml => ".xml",
			FileType.JsonKxv1 => ".json",
			FileType.JsonKxv2 => ".json",
			_ => throw new ArgumentOutOfRangeException(nameof(fileType))
		};

		var fileNameWithExtension = Path.GetExtension(fileName).Equals(extension, StringComparison.OrdinalIgnoreCase)
			? fileName
			: fileName + extension;

		var fullPath = Path.Combine(directoryPath, fileNameWithExtension);

		var content = fileType switch
		{
			FileType.Xml => "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Root>\r\n</Root>",
			FileType.JsonKxv1 => "{\r\n  \"version\": \"kxv1\"\r\n}",
			FileType.JsonKxv2 => "{\r\n  \"version\": \"kxv2\"\r\n}",
			_ => string.Empty
		};

		await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8, cancellationToken);
		return fullPath;
	}

	public Task<string> CreateDirectoryAsync(string parentPath, string directoryName,
		CancellationToken cancellationToken = default)
	{
		if (!ValidateDirectoryName(parentPath, directoryName, out var errorMessage))
			throw new InvalidOperationException(errorMessage);

		var fullPath = Path.Combine(parentPath, directoryName);
		Directory.CreateDirectory(fullPath);
		return Task.FromResult(fullPath);
	}

	public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
	{
		if (!File.Exists(filePath))
			throw new FileNotFoundException("File not found.", filePath);

		File.Delete(filePath);
		return Task.CompletedTask;
	}

	public Task DeleteDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default)
	{
		if (IsRootDataFolder(directoryPath))
			throw new InvalidOperationException("Cannot delete the root Data folder.");

		if (!Directory.Exists(directoryPath))
			throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

		Directory.Delete(directoryPath, true);
		return Task.CompletedTask;
	}

	public async Task CreateFileInArchiveAsync(string archivePath, string fileName,
		CancellationToken cancellationToken = default)
	{
		if (!File.Exists(archivePath))
			throw new FileNotFoundException("Archive not found.", archivePath);

		var extension = Path.GetExtension(archivePath).ToLowerInvariant();
		if (extension != ".zip" && extension != ".taco")
			throw new InvalidOperationException("Only .zip and .taco archives are supported.");

		var fileNameWithExtension = Path.GetExtension(fileName).Equals(".xml", StringComparison.OrdinalIgnoreCase)
			? fileName
			: fileName + ".xml";

		await using var fileStream = new FileStream(archivePath, FileMode.Open, FileAccess.ReadWrite);
		using var archive = new ZipArchive(fileStream, ZipArchiveMode.Update);

		// Check if entry already exists
		var existingEntry = archive.GetEntry(fileNameWithExtension);
		if (existingEntry != null)
			throw new InvalidOperationException($"File '{fileNameWithExtension}' already exists in the archive.");

		var entry = archive.CreateEntry(fileNameWithExtension);
		await using var entryStream = entry.Open();
		var content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Root>\r\n</Root>"u8.ToArray();
		await entryStream.WriteAsync(content, cancellationToken);
	}

	public bool ValidateFileName(string directoryPath, string fileName, out string? errorMessage)
	{
		errorMessage = null;

		if (string.IsNullOrWhiteSpace(fileName))
		{
			errorMessage = "File name cannot be empty.";
			return false;
		}

		if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
		{
			errorMessage = "File name contains invalid characters.";
			return false;
		}

		var extension = Path.GetExtension(fileName);
		if (!string.IsNullOrEmpty(extension) && !FileExtension.AllowedExtensions.Contains(extension))
		{
			errorMessage =
				$"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", FileExtension.AllowedExtensions)}";
			return false;
		}

		if (!Directory.Exists(directoryPath))
		{
			errorMessage = "Directory does not exist.";
			return false;
		}

		// Check for conflicts (with any allowed extension)
		var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
		foreach (var allowedExtension in FileExtension.AllowedExtensions)
		{
			var testPath = Path.Combine(directoryPath, fileNameWithoutExtension + allowedExtension);
			if (File.Exists(testPath))
			{
				errorMessage = $"A file with name '{fileNameWithoutExtension}' already exists.";
				return false;
			}
		}

		return true;
	}

	public bool ValidateDirectoryName(string parentPath, string directoryName, out string? errorMessage)
	{
		errorMessage = null;

		if (string.IsNullOrWhiteSpace(directoryName))
		{
			errorMessage = "Directory name cannot be empty.";
			return false;
		}

		if (directoryName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
		{
			errorMessage = "Directory name contains invalid characters.";
			return false;
		}

		if (!Directory.Exists(parentPath))
		{
			errorMessage = "Parent directory does not exist.";
			return false;
		}

		var fullPath = Path.Combine(parentPath, directoryName);
		if (Directory.Exists(fullPath))
		{
			errorMessage = "A directory with this name already exists.";
			return false;
		}

		return true;
	}

	public bool IsRootDataFolder(string path)
	{
		var normalizedPath = Path.GetFullPath(path);
		var normalizedDataFolder = Path.GetFullPath(_dataFolderPath);
		return string.Equals(normalizedPath, normalizedDataFolder, StringComparison.OrdinalIgnoreCase);
	}
}