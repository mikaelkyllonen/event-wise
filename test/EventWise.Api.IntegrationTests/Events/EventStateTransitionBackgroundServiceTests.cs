using Bogus;

using EventWise.Api.Common;
using EventWise.Api.Events;
using EventWise.Api.IntegrationTests.Infrastructure;
using EventWise.Api.Users;

using Microsoft.Extensions.Logging;

using NSubstitute;

namespace EventWise.Api.IntegrationTests.Events;

public sealed class EventStateTransitionBackgroundServiceTests : BaseIntegrationTests
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<EventStateTransitionBackgroundService> _logger;

    public EventStateTransitionBackgroundServiceTests(WebAppFactory factory) : base(factory)
    {
        _logger = Substitute.For<ILogger<EventStateTransitionBackgroundService>>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task Starts_published_events_when_start_time_reached()
    {
        // Arrange
        var faker = new Faker();
        var hostId = Guid.NewGuid();
        var @event = UserEvent.Create(
            hostId,
            faker.Random.String2(10),
            faker.Random.String2(20),
            faker.Locale,
            faker.Random.Int(2, 10),
            _dateTimeProvider.UtcNow,
            _dateTimeProvider.UtcNow,
            _dateTimeProvider.UtcNow).Value;
        var user = User.Create(hostId, faker.Person.FirstName, faker.Person.LastName, faker.Person.Email).Value;

        DbContext.Users.Add(user);
        DbContext.Events.Add(@event);
        await DbContext.SaveChangesAsync();

        // Act
        var service = new EventStateTransitionBackgroundService(Scope.ServiceProvider, _dateTimeProvider, _logger);
        await service.TransitionEventStatesAsync(DbContext, default);

        // Assert
        var updatedEvent = await DbContext.Events.FindAsync(@event.Id);
        Assert.Equal(EventState.InProgress, updatedEvent!.EventState);
    }

    [Fact]
    public async Task Completes_in_progress_events_when_end_time_reached()
    {
        // Arrange
        var faker = new Faker();
        var hostId = Guid.NewGuid();
        var @event = UserEvent.Create(
            hostId,
            faker.Random.String2(10),
            faker.Random.String2(20),
            faker.Locale,
            faker.Random.Int(2, 10),
            _dateTimeProvider.UtcNow,
            _dateTimeProvider.UtcNow,
            _dateTimeProvider.UtcNow).Value;
        var user = User.Create(hostId, faker.Person.FirstName, faker.Person.LastName, faker.Person.Email).Value;
        @event.Start();

        DbContext.Users.Add(user);
        DbContext.Events.Add(@event);
        await DbContext.SaveChangesAsync();

        // Act
        var service = new EventStateTransitionBackgroundService(Scope.ServiceProvider, _dateTimeProvider, _logger);
        await service.TransitionEventStatesAsync(DbContext, default);

        // Assert
        var updatedEvent = await DbContext.Events.FindAsync(@event.Id);
        Assert.Equal(EventState.Completed, updatedEvent!.EventState);
    }
}