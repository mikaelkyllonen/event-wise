using EventWise.Api.Events;
using EventWise.Api.Users;

namespace EventWise.Api.UnitTests.Events;

public sealed class EventTests
{
    [Test]
    public async Task Cannot_create_event_in_the_past()
    {
        // Arrange
        var pastStartTime = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = UserEvent.Create(
            Guid.NewGuid(),
            "Event",
            "Description",
            "Location",
            10,
            pastStartTime,
            null);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.StartTimeInPast);
    }

    [Test]
    public async Task Cannot_create_event_with_end_time_before_start_time()
    {
        // Arrange
        var startTimeNow = DateTime.UtcNow;
        var endTimePast = DateTime.UtcNow.AddHours(-1);

        // Act
        var result = UserEvent.Create(
            Guid.NewGuid(),
            "Event",
            "Description",
            "Location",
            10,
            startTimeNow,
            endTimePast);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.StartTimeAfterEndTime);
    }

    [Test]
    public async Task Cannot_create_event_with_max_participants_less_than_one()
    {
        // Arrange
        var participantCount = 0;

        // Act
        var result = UserEvent.Create(
            Guid.NewGuid(),
            "Event",
            "Description",
            "Location",
            participantCount,
            DateTime.UtcNow.AddHours(1),
            null);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.MaxParticipantsLessThanOne);
    }

    [Test]
    public async Task Cannot_create_event_with_max_participants_greater_than_allowed_for_user_events()
    {
        // Arrange
        var participantCount = UserEvent.MaxParticipantsForUserEvents + 1;

        // Act
        var result = UserEvent.Create(
            Guid.NewGuid(),
            "Event",
            "Description",
            "Location",
            participantCount,
            DateTime.UtcNow.AddHours(1),
            null);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.MaxParticipantsGreaterThanMax);
    }

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
        var result = userEvent.Participate(user);

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

        userEvent.Participate(user);

        // Act
        var result = userEvent.Participate(user);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.EventFull);
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

        userEvent.Participate(user);

        // Act
        var result = userEvent.Participate(user);

        // Assert
        await Assert.That(userEvent.Participants.Count).IsEqualTo(1);
        await Assert.That(result.IsFailure).IsTrue();
    }

    [Test]
    public async Task Cannot_participate_in_canceled_event()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "User", "Test", "test@localhost.com").Value;
        var userEvent = UserEvent.Create(
            Guid.NewGuid(),
            "Event",
            "Description",
            "Location",
            1,
            DateTime.UtcNow.AddHours(1),
            null).Value;

        userEvent.Cancel();

        // Act
        var result = userEvent.Participate(user);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.EventCanceled);
    }

    [Test]
    public async Task Cannot_participate_in_completed_event()
    {
        // Arrange
        var user = User.Create(Guid.NewGuid(), "User", "Test", "test@localhost.com").Value;
        var userEvent = UserEvent.Create(
            Guid.NewGuid(),
            "Event",
            "Description",
            "Location",
            1,
            DateTime.UtcNow.AddHours(1),
            null).Value;

        userEvent.Complete();

        // Act
        var result = userEvent.Participate(user);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.EventEnded);
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
        var result = userEvent.Participate(host);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(EventErrors.HostCannotParticipate);
    }
}