
using EventWise.Api.Common;

using Microsoft.EntityFrameworkCore;

namespace EventWise.Api.Events;

public sealed class EventStateTransitionBackgroundService(
    IServiceProvider serviceProvider,
    IDateTimeProvider dateTimeProvider) : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var currentUtcTime = _dateTimeProvider.UtcNow;

            await StartEvents(currentUtcTime, dbContext, stoppingToken);
            await CompleteEvents(currentUtcTime, dbContext, stoppingToken);

            await dbContext.SaveChangesAsync(stoppingToken);

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private static async Task CompleteEvents(DateTime currentUtcTime, ApplicationDbContext dbContext, CancellationToken stoppingToken)
    {
        var eventsToComplete = await dbContext.Events
                        .Where(e => e.EventState == EventState.InProgress && e.EndTimeUtc <= currentUtcTime)
                        .ToListAsync(cancellationToken: stoppingToken);

        foreach (var @event in eventsToComplete)
        {
            @event.Complete();
        }
    }

    private static async Task StartEvents(DateTime currentUtcTime, ApplicationDbContext dbContext, CancellationToken stoppingToken)
    {
        var eventsToStart = await dbContext.Events
                        .Where(e => e.EventState == EventState.Published && e.StartTimeUtc <= currentUtcTime)
                        .ToListAsync(cancellationToken: stoppingToken);

        foreach (var @event in eventsToStart)
        {
            @event.Start();
        }
    }
}
