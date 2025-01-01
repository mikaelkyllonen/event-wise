using EventWise.Api.Common;
using EventWise.Api.Users;

namespace EventWise.Api.UnitTests.Users;

public sealed class UserTests()
{
    [Test]
    public async Task Cannot_create_user_with_invalid_email()
    {
        // Arrange
        var emailWithoutAt = "testlocalhost.com";

        // Act
        var result = User.Create(Guid.NewGuid(), "test", "user", emailWithoutAt);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(UserErrors.InvalidEmail);
    }

    [Test]
    [MethodDataSource(typeof(UserTests), nameof(IncompleteUserData))]
    public async Task Cannot_create_user_with_missing_required_fields(string firstName, string lastName, string email, Error expectedError)
    {
        // Act
        var result = User.Create(Guid.NewGuid(), firstName, lastName, email);

        // Assert
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsEqualTo(expectedError);
    }

    public static IEnumerable<Func<(string firstName, string lastName, string email, Error error)>> IncompleteUserData()
    {
        yield return () => ("", "user", "test@localhost.com", UserErrors.FirstNameRequired);
        yield return () => ("   ", "user", "test@localhost.com", UserErrors.FirstNameRequired);
        yield return () => ("test", "", "test@localhost.com", UserErrors.LastNameRequired);
        yield return () => ("test", "   ", "test@localhost.com", UserErrors.LastNameRequired);
        yield return () => ("test", "user", "", UserErrors.EmailRequired);
        yield return () => ("test", "user", "   ", UserErrors.EmailRequired);
    }
}