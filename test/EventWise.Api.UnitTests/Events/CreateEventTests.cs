using EventWise.Api.Events;
using EventWise.Api.Users;

namespace EventWise.Api.UnitTests.Events;

public sealed class CreateEventTests
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
}