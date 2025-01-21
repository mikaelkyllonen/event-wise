using System.Text;

namespace EventWise.Api.FunctionalTests.Infrastructure;

public static class TestJwtConfiguration
{
    public static JwtBearerOptions CreateTestOptions() =>
        new()
        {
            ValidAuthority = "https://event-wise-test-authority",
            ValidAudiences = ["test-audience"],
            ValidIssuers = ["https://event-wise-test-issuer"],
            SigningKeys =
            [
                new SigningKeys
                {
                    Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()))
                }
            ]
        };

    public static Dictionary<string, string?> ToConfigurationDictionary(JwtBearerOptions jwtOptions) =>
        new()
        {
            ["Authentication:Schemes:Bearer:ValidAuthority"] = jwtOptions.ValidAuthority,
            ["Authentication:Schemes:Bearer:ValidAudiences:0"] = jwtOptions.ValidAudiences[0],
            ["Authentication:Schemes:Bearer:ValidIssuers:0"] = jwtOptions.ValidIssuers[0],
            ["Authentication:Schemes:Bearer:SigningKeys:0:Value"] = jwtOptions.SigningKeys[0].Value
        };
}
