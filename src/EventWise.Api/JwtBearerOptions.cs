namespace EventWise.Api;

public sealed class JwtBearerOptions
{
    public string ValidAuthority { get; set; } = string.Empty;
    public string[] ValidAudiences { get; set; } = [];
    public string[] ValidIssuers { get; set; } = [];
    public SigningKeys[] SigningKeys { get; set; } = [];
}

public sealed class SigningKeys
{
    public string Value { get; set; } = string.Empty;
}