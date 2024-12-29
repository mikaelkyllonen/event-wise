using EventWise.Api.Common;

namespace EventWise.Api.Events;

public static class UserErrors
{
    public static readonly Error FirstNameRequired = new(
        "User.FirstNameRequired",
        "First name is required");

    public static readonly Error LastNameRequired = new(
        "User.LastNameRequired",
        "Last name is required");

    public static readonly Error EmailRequired = new(
        "User.EmailRequired",
        "Email is required");
}