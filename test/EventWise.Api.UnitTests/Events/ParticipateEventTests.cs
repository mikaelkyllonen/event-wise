using EventWise.Api.Events;
using EventWise.Api.Users;

namespace EventWise.Api.UnitTests.Events;

public sealed class ParticipateEventTests
{
    [Test]
    public async Task Can_participate_in_event_with_available_spots()
    {
        // Arrange
        var maxParticipants = 1;
        var user = User.Create(Guid.NewGuid(), "User", "Test", "test@localhost.com").Value;
        var userEvent = UserEvent.Create(
            Guid.NewGuid(),
            "Event",
            "Description",
            "Location",
            maxParticipants,
            DateTime.UtcNow.AddHours(1),
            null).Value;

        // Act
        var result = userEvent.Participate(user.Id);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
    }

    [Test]
    public async Task Cannot_participate_in_full_event()
    {
        // Arrange
        var maxParticipants = 1;
        var user = User.Create(Guid.NewGuid(), "User", "Test", "test@localhost.com").Value;
        var userEvent = UserEvent.Create(
            Guid.NewGuid(),
            "Event",
            "Description",
            "Location",
            maxParticipants,
            DateTime.UtcNow.AddHours(1),
            null).Value;

        userEvent.Participate(user.Id);

        // Act
        var result = userEvent.Participate(user.Id);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.Participation.EventFull);
    }

    [Test]
    public async Task Cannot_participate_in_event_twice()
    {
        // Arrange
        var maxParticipants = 3;
        var user = User.Create(Guid.NewGuid(), "User", "Test", "test@localhost.com").Value;
        var userEvent = UserEvent.Create(
            Guid.NewGuid(),
            "Event",
            "Description",
            "Location",
            maxParticipants,
            DateTime.UtcNow.AddHours(1),
            null).Value;

        userEvent.Participate(user.Id);

        // Act
        var result = userEvent.Participate(user.Id);

        // Assert
        await Assert.That(userEvent.Participants.Count).IsEqualTo(1);
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task Cannot_participate_in_canceled_event()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "User", "Test", "test@localhost.com").Value;
        var userEvent = TestData.CreateEventWith(EventState.Canceled);

        // Act
        var result = userEvent.Participate(user.Id);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.Participation.EventCanceled);
    }

    [Test]
    public async Task Cannot_participate_in_completed_event()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "User", "Test", "test@localhost.com").Value;
        var userEvent = TestData.CreateEventWith(EventState.Completed);

        // Act
        var result = userEvent.Participate(user.Id);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.Participation.EventCompleted);
    }

    [Test]
    public async Task Host_cannot_join_as_participant_in_their_own_event()
    {
        // Arrange
        var hostId = Guid.NewGuid();
        var host = User.Create(hostId, "User", "Test", "test@localhost.com").Value;
        var userEvent = UserEvent.Create(
            hostId,
            "Event",
            "Description",
            "Location",
            1,
            DateTime.UtcNow.AddHours(1),
            null).Value;

        // Act
        var result = userEvent.Participate(host.Id);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.Participation.HostCannotParticipate);
    }
}