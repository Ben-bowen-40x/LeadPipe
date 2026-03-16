using LeadPipe.Infrastructure.Interfaces.Core;

namespace LeadPipe.Infrastructure.Service;

internal class SystemClock : IClock { public DateTimeOffset UtcNow => DateTimeOffset.UtcNow; }
