namespace KXMapStudio.Core.Models;

public sealed record GitHubRelease(
	[property: JsonPropertyName("tag_name")]
	string TagName,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("body")] string Body,
	[property: JsonPropertyName("html_url")]
	string HtmlUrl
);