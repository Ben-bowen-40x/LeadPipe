using LeadPipe.Infrastructure.Interface.Core;

namespace LeadPipe.Infrastructure.Service;

internal class SystemClock : IClock { public DateTimeOffset UtcNow => DateTimeOffset.UtcNow; }
