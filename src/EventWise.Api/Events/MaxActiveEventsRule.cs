using EventWise.Api.Common;
using EventWise.Api.Users;

using Microsoft.EntityFrameworkCore;

namespace EventWise.Api.Events;

public sealed class MaxActiveEventsRule(ApplicationDbContext context) : IRule<User>
{
    // This can be later moved to configuration
    public const int MaxActiveEvents = 3;
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> CheckAsync(User entity)
    {
        var activeEventsCount = await _context.Events
            .CountAsync(e => e.HostId == entity.Id && (e.EventState == EventState.Published || e.EventState == EventState.InProgress));

        if (activeEventsCount >= MaxActiveEvents)
        {
            return Result.Failure(EventErrors.MaxActiveEvents);
        }

        return Result.Success();
    }
}