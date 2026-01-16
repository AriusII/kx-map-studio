namespace KXMapStudio.Core.Abstractions.Services;

public interface IArchiveService
{
    Task<IEnumerable<string>> GetXmlFilesAsync(byte[] tacoData, CancellationToken cancellationToken = default);

    Task<XDocument?> LoadXmlFileAsync(byte[] tacoData, string fileName, CancellationToken cancellationToken = default);

    TacoMarkerPackModel ParseMarkerPack(XDocument document);

    Task<IReadOnlyList<TacoMarkerPackModel>> LoadMarkerPacksAsync(byte[] archiveData,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TacoMarkerPackModel>> LoadMarkerPacksAsync(string path,
        CancellationToken cancellationToken = default);
}