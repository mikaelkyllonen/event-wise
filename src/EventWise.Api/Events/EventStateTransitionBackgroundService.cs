
using Microsoft.EntityFrameworkCore;

namespace EventWise.Api.Events;

public sealed class EventStateTransitionBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var currentUtcTime = DateTime.UtcNow;

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var eventsToStart = await dbContext.Events
                .Where(e => e.EventState == EventState.Published && e.StartTimeUtc <= currentUtcTime)
                .ToListAsync(cancellationToken: stoppingToken);

            foreach (var @event in eventsToStart)
            {
                @event.Start();
            }

            var eventsToComplete = await dbContext.Events
                .Where(e => e.EventState == EventState.InProgress && e.EndTimeUtc <= currentUtcTime)
                .ToListAsync(cancellationToken: stoppingToken);

            foreach (var @event in eventsToComplete)
            {
                @event.Complete();
            }

            await dbContext.SaveChangesAsync(stoppingToken);

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}
