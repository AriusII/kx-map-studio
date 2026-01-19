namespace KXMapStudio.Libs.Abstractions.Services.FilePreview;

public interface IFilePreviewService
{
	Task<FilePreviewResult> BuildPreviewAsync(string fullPath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Expands an archive XML entry leaf into XML child nodes.
	/// </summary>
	/// <param name="archiveXmlNode">A node representing an XML file inside an archive.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A list of XML child nodes to be attached under the entry node.</returns>
	Task<IReadOnlyList<PreviewTreeNodeModel>> ExpandArchiveXmlAsync(PreviewTreeNodeModel archiveXmlNode,
		CancellationToken cancellationToken = default);
}