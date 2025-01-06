using EventWise.Api.Common;

using Microsoft.EntityFrameworkCore;

namespace EventWise.Api.Events;

public sealed class MaxActiveEventsRule(
    ApplicationDbContext context,
    IDateTimeProvider dateTimeProvider) : IRule<UserEvent>
{
    // This can be later moved to configuration
    private const int _maxActiveEvents = 3;
    private readonly ApplicationDbContext _context = context;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<Result> CheckAsync(UserEvent entity)
    {
        var activeEventsCount = await _context.Events
            .CountAsync(e => e.HostId == entity.HostId && e.EndTimeUtc > _dateTimeProvider.UtcNow);

        if (activeEventsCount >= _maxActiveEvents)
        {
            return Result.Failure(EventErrors.MaxActiveEvents);
        }

        return Result.Success();
    }
}