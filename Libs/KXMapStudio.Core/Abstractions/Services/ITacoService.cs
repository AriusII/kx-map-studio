namespace KXMapStudio.Core.Abstractions.Services;

/// <summary>
///     Service responsible for handling TacO marker pack files (.taco).
/// </summary>
public interface ITacoService
{
    /// <summary>
    ///     Lists all XML files within the taco archive data.
    /// </summary>
    /// <param name="tacoData">The raw bytes of the .taco archive.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of XML file paths found in the archive.</returns>
    Task<IEnumerable<string>> GetXmlFilesAsync(byte[] tacoData, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Loads and parses an XML file from the taco archive data.
    /// </summary>
    /// <param name="tacoData">The raw bytes of the .taco archive.</param>
    /// <param name="fileName">The path of the XML file within the archive.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed XDocument, or null if not found.</returns>
    Task<XDocument?> LoadXmlFileAsync(byte[] tacoData, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Parses an XDocument into a typed TacoMarkerPack.
    /// </summary>
    /// <param name="document">The XDocument to parse.</param>
    /// <returns>A typed representation of the marker pack.</returns>
    TacoMarkerPack ParseMarkerPack(XDocument document);
}