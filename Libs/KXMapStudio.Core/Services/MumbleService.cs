namespace KXMapStudio.Core.Services;

public sealed record MumbleService(IGw2Client Gw2Client) : IMumbleService
{
}