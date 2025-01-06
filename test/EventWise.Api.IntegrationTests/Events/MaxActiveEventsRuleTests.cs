using Bogus;

using EventWise.Api.Events;
using EventWise.Api.IntegrationTests.Infrastructure;
using EventWise.Api.Users;

using Microsoft.Extensions.Logging;

using NSubstitute;

namespace EventWise.Api.IntegrationTests.Events;

public sealed class MaxActiveEventsRuleTests(WebAppFactory factory) : BaseIntegrationTests(factory)
{
    private readonly ILogger<MaxActiveEventsRule> _logger = Substitute.For<ILogger<MaxActiveEventsRule>>();

    [Fact]
    public async Task Cannot_create_event_when_user_has_max_active_events()
    {
        // Arrange
        var maxActiveEvents = MaxActiveEventsRule.MaxActiveEvents;
        var faker = new Faker();
        var user = User.Create(Guid.NewGuid(), faker.Person.FirstName, faker.Person.LastName, faker.Person.Email).Value;
        var events = Enumerable.Range(0, maxActiveEvents)
            .Select(_ => CreateEvent(faker, user.Id))
            .ToList();

        DbContext.Users.Add(user);
        DbContext.Events.AddRange(events);
        await DbContext.SaveChangesAsync();

        var rule = new MaxActiveEventsRule(DbContext, _logger);

        // Act
        var result = await rule.CheckAsync(user);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(EventErrors.MaxActiveEvents, result.Error);
        _logger.VerifyLogArguments(LogLevel.Warning, [user.Id, maxActiveEvents]);
    }

    [Fact]
    public async Task Can_create_event_when_user_has_less_than_max_active_events()
    {
        // Arrange
        var maxActiveEventsMinusOne = MaxActiveEventsRule.MaxActiveEvents - 1;
        var faker = new Faker();
        var user = User.Create(Guid.NewGuid(), faker.Person.FirstName, faker.Person.LastName, faker.Person.Email).Value;
        var events = Enumerable.Range(0, maxActiveEventsMinusOne)
            .Select(_ => CreateEvent(faker, user.Id))
            .ToList();

        DbContext.Users.Add(user);
        DbContext.Events.AddRange(events);
        await DbContext.SaveChangesAsync();

        var rule = new MaxActiveEventsRule(DbContext, _logger);

        // Act
        var result = await rule.CheckAsync(user);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Completed_events_do_not_count_towards_max_active_events()
    {
        // Arrange
        var faker = new Faker();
        var user = User.Create(Guid.NewGuid(), faker.Person.FirstName, faker.Person.LastName, faker.Person.Email).Value;
        var activeEvents = Enumerable.Range(0, MaxActiveEventsRule.MaxActiveEvents - 1)
            .Select(_ => CreateEvent(faker, user.Id))
            .ToList();

        var completedEvent = CreateEvent(faker, user.Id);
        completedEvent.Start();
        completedEvent.Complete();

        var canceledEvent = CreateEvent(faker, user.Id);
        canceledEvent.Cancel();

        DbContext.Users.Add(user);
        DbContext.Events.AddRange(activeEvents);
        DbContext.Events.Add(completedEvent);
        DbContext.Events.Add(canceledEvent);
        await DbContext.SaveChangesAsync();

        var rule = new MaxActiveEventsRule(DbContext, _logger);

        // Act
        var result = await rule.CheckAsync(user);

        // Assert
        Assert.True(result.IsSuccess);
    }

    private static UserEvent CreateEvent(Faker faker, Guid hostId) =>
        UserEvent.Create(
            hostId,
            faker.Random.String2(10),
            faker.Random.String2(20),
            faker.Locale,
            faker.Random.Int(2, 10),
            faker.Date.Soon(),
            faker.Date.Future()).Value;
}