using Microsoft.Extensions.Logging;

using NSubstitute;

namespace EventWise.Api.IntegrationTests;

internal static class TestHelper
{
    internal static void VerifyLogArguments(this ILogger logger, LogLevel logLevel, object[] arguments)
    {
        logger.Received().Log(
            logLevel: logLevel,
            eventId: Arg.Any<EventId>(),
            state: Arg.Is<IReadOnlyList<KeyValuePair<string, object>>>(list =>
                arguments.All(arg => list.Any(kvp => kvp.Value.ToString() == arg.ToString()))),
            exception: null,
            formatter: Arg.Any<Func<IReadOnlyList<KeyValuePair<string, object>>, Exception?, string>>());
    }
}