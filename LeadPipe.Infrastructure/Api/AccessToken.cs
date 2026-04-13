namespace LeadPipe.Infrastructure.Api;

internal readonly record struct AccessToken(
    string Value,
    long ExpiresAtUnixUtc)
{
    public bool IsExpired(long now, long bufferSeconds = 0) =>
        ExpiresAtUnixUtc <= now + bufferSeconds;

    public long GetTtlSeconds(long now, long bufferSeconds = 0) =>
        ExpiresAtUnixUtc - now - bufferSeconds;
}
