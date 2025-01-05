using Bogus;

using EventWise.Api.Events;
using EventWise.Api.IntegrationTests.Infrastructure;
using EventWise.Api.Users;

namespace EventWise.Api.IntegrationTests.Events;

public sealed class EventStateTransitionBackgroundServiceTests(WebAppFactory factory) : BaseIntegrationTests(factory)
{
    [Fact]
    public async Task Starts_published_events_when_start_time_reached()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var faker = new Faker();
        var hostId = Guid.NewGuid();
        var @event = UserEvent.Create(
            hostId,
            faker.Random.String2(10),
            faker.Random.String2(20),
            faker.Locale,
            faker.Random.Int(2, 10),
            startTime,
            faker.Date.Future());
        if (@event.IsFailure)
        {
            throw new InvalidOperationException(@event.Error.ToString());
        }

        var user = User.Create(hostId, faker.Person.FirstName, faker.Person.LastName, faker.Person.Email).Value;

        DbContext.Users.Add(user);
        DbContext.Events.Add(@event.Value);
        await DbContext.SaveChangesAsync();

        // Act
        var service = new EventStateTransitionBackgroundService(Scope.ServiceProvider);
        await service.StartAsync(default);
        await service.ExecuteTask!;

        // Assert
        var updatedEvent = await DbContext.Events.FindAsync(@event.Value.Id);
        Assert.Equal(EventState.InProgress, updatedEvent!.EventState);
    }

    [Fact]
    public async Task Completes_in_progress_events_when_end_time_reached()
    {

    }
}
