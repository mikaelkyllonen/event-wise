using EventWise.Api.Events;
using EventWise.Api.Users;

namespace EventWise.Api.UnitTests.Events;

public sealed class LeaveEventTests
{
    [Test]
    public async Task Can_leave_when_participating()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "test", "user", "test@localhost.com").Value;
        var @event = TestData.CreateEventWithParticipant(user, EventState.Published);

        // Act
        var result = @event.Leave(user.Id);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(@event.Participants).IsEmpty();
    }

    [Test]
    public async Task Cannot_leave_when_not_participating()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "test", "user", "test@localhost.com").Value;
        var @event = TestData.CreateEventWith(EventState.Published);

        // Act
        var result = @event.Leave(user.Id);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.Participation.NotParticipating);
    }

    [Test]
    [Arguments(EventState.Completed)]
    [Arguments(EventState.Canceled)]
    public async Task Cannot_leave_when_event_has_finished(EventState eventState)
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "test", "user", "test@localhost.com").Value;
        var @event = TestData.CreateEventWithParticipant(user, eventState);

        // Act
        var result = @event.Leave(user.Id);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(@event.Participants).IsNotEmpty();
    }
}