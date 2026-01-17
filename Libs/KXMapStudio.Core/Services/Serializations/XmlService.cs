using System.Text;
using System.Xml;
using KXMapStudio.Core.Models.Xml.Taco.Dtos;

namespace KXMapStudio.Core.Services.Serializations;

public sealed record XmlService : IXmlService
{
	public async Task<TacoMarkerPackModel?> LoadTacoMarkerPackAsync(string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);
		if (!File.Exists(path)) return null;

		await using var stream = File.OpenRead(path);
		var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

		return ParseMarkerPack(document);
	}

	public async Task SaveTacoMarkerPackAsync(TacoMarkerPackModel model, string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		var directory = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

		var dto = TacoMapper.MapToDto(model);
		var serializer = new XmlSerializer(typeof(TacoOverlayDataDto));

		var settings = new XmlWriterSettings
		{
			Indent = true,
			Encoding = new UTF8Encoding(false),
			Async = true
		};

		await using var stream = File.Create(path);
		await using var writer = XmlWriter.Create(stream, settings);
		serializer.Serialize(writer, dto);
	}

	private static TacoMarkerPackModel ParseMarkerPack(XDocument document)
	{
		try
		{
			var serializer = new XmlSerializer(typeof(TacoOverlayDataDto));
			using var reader = document.CreateReader();
			var dto = (TacoOverlayDataDto?)serializer.Deserialize(reader);

			return dto != null
				? TacoMapper.MapToDomain(dto)
				: new TacoMarkerPackModel([], [], []);
		}
		catch (Exception)
		{
			return new TacoMarkerPackModel([], [], []);
		}
	}
}