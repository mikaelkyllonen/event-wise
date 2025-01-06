using EventWise.Api.Common;
using EventWise.Api.Events;

using Microsoft.EntityFrameworkCore;

namespace EventWise.Api.Users;

public sealed class MaxActiveEventsRule(ApplicationDbContext context, ILogger<MaxActiveEventsRule> logger) : IRule<User>
{
    // This can be later moved to configuration
    public const int MaxActiveEvents = 3;
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<MaxActiveEventsRule> _logger = logger;

    public async Task<Result> CheckAsync(User entity)
    {
        var activeEventsCount = await _context.Events
            .CountAsync(e => e.HostId == entity.Id && (e.EventState == EventState.Published || e.EventState == EventState.InProgress));

        if (activeEventsCount >= MaxActiveEvents)
        {
            _logger.LogWarning("User {UserId} has reached the maximum number of active events ({MaxActiveEvents}).", entity.Id, MaxActiveEvents);
            return Result.Failure(EventErrors.MaxActiveEvents);
        }

        return Result.Success();
    }
}