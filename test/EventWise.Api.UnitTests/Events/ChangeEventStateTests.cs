using EventWise.Api.Events;

namespace EventWise.Api.UnitTests.Events;

public sealed class ChangeEventStateTests
{
    [Test]
    public async Task Can_start_published_event()
    {
        // Arrange
        var @event = TestData.CreateEventWith(EventState.Published);

        // Act
        var result = @event.Start();

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(@event.EventState).IsEqualTo(EventState.InProgress);
    }

    [Test]
    public async Task Can_cancel_published_event()
    {
        // Arrange
        var @event = TestData.CreateEventWith(EventState.Published);

        // Act
        var result = @event.Cancel();

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(@event.EventState).IsEqualTo(EventState.Canceled);
    }

    [Test]
    public async Task Can_complete_in_progress_event()
    {
        // Arrange
        var @event = TestData.CreateEventWith(EventState.InProgress);

        // Act
        var result = @event.Complete();

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(@event.EventState).IsEqualTo(EventState.Completed);
    }

    [Test]
    public async Task Can_cancel_in_progress_event()
    {
        // Arrange
        var @event = TestData.CreateEventWith(EventState.InProgress);

        // Act
        var result = @event.Cancel();

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(@event.EventState).IsEqualTo(EventState.Canceled);
    }

    [Test]
    [Arguments(EventState.InProgress)]
    [Arguments(EventState.Completed)]
    [Arguments(EventState.Canceled)]
    public async Task Cannot_start_event_when_not_published(EventState eventState)
    {
        // Arrange
        var @event = TestData.CreateEventWith(eventState);

        // Act
        var result = @event.Start();

        // Assert
        await Assert.That(result.Error).IsEqualTo(EventErrors.State.CannotStart);
        await Assert.That(@event.EventState).IsEqualTo(eventState);
    }

    [Test]
    [Arguments(EventState.Published)]
    [Arguments(EventState.Completed)]
    [Arguments(EventState.Canceled)]
    public async Task Cannot_complete_event_when_not_in_progress(EventState eventState)
    {
        // Arrange
        var @event = TestData.CreateEventWith(eventState);

        // Act
        var result = @event.Complete();

        // Assert
        await Assert.That(result.Error).IsEqualTo(EventErrors.State.CannotComplete);
        await Assert.That(@event.EventState).IsEqualTo(eventState);
    }

    [Test]
    [Arguments(EventState.Completed)]
    [Arguments(EventState.Canceled)]
    public async Task Cannot_cancel_event_when_in_final_state(EventState eventState)
    {
        // Arrange
        var @event = TestData.CreateEventWith(eventState);

        // Act
        var result = @event.Cancel();

        // Assert
        await Assert.That(result.Error).IsEqualTo(EventErrors.State.CannotCancel);
        await Assert.That(@event.EventState).IsEqualTo(eventState);
    }
}