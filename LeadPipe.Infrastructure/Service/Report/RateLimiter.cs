namespace LeadPipe.Infrastructure.Service.Report;

public sealed class RateLimiter(int maxRequests, TimeSpan perTimeSpan)
{
    private readonly int _maxRequests = maxRequests;
    private readonly TimeSpan _perTimeSpan = perTimeSpan;
    private readonly Queue<DateTime> _timestamps = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task WaitForAvailabilityAsync()
    {
        await _lock.WaitAsync();
        try
        {
            DateTime now = DateTime.UtcNow;

            // Remove timestamps older than the window
            while (_timestamps.Count > 0 && now - _timestamps.Peek() > _perTimeSpan)
                _timestamps.Dequeue();

            while (_timestamps.Count >= _maxRequests)
            {
                var waitTime = _perTimeSpan - (now - _timestamps.Peek());
                if (waitTime > TimeSpan.Zero)
                    await Task.Delay(waitTime);
                while (_timestamps.Count > 0 && DateTime.UtcNow - _timestamps.Peek() > _perTimeSpan)
                    _timestamps.Dequeue();
            }

            _timestamps.Enqueue(DateTime.UtcNow);
        }
        finally
        {
            _lock.Release();
        }
    }
}
