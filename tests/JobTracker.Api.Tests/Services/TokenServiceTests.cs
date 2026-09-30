using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JobTracker.Api.Models;
using JobTracker.Api.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace JobTracker.Api.Tests.Services;

public class TokenServiceTests
{
    private const string Key = "unit-test-signing-key-that-is-long-enough-for-hs256";

    private static IConfiguration BuildConfig(string? expiresMinutes = "60") =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = Key,
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience",
                ["Jwt:ExpiresMinutes"] = expiresMinutes
            })
            .Build();

    private static ApplicationUser NewUser(string? displayName = "Jane") =>
        new() { Id = "user-1", Email = "jane@acme.test", DisplayName = displayName };

    [Fact]
    public void CreateToken_ContainsUserClaimsIssuerAndAudience()
    {
        var sut = new TokenService(BuildConfig());

        var (token, _) = sut.CreateToken(NewUser());

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("test-issuer", jwt.Issuer);
        Assert.Contains("test-audience", jwt.Audiences);
        Assert.Equal("user-1", jwt.Subject);
        Assert.Equal("user-1", jwt.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal("jane@acme.test", jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal("Jane", jwt.Claims.Single(c => c.Type == "displayName").Value);
    }

    [Fact]
    public void CreateToken_OmitsDisplayNameClaim_WhenNotSet()
    {
        var sut = new TokenService(BuildConfig());

        var (token, _) = sut.CreateToken(NewUser(displayName: null));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.DoesNotContain(jwt.Claims, c => c.Type == "displayName");
    }

    [Fact]
    public void CreateToken_ExpiresAfterConfiguredMinutes()
    {
        var sut = new TokenService(BuildConfig("30"));

        var (_, expiresAt) = sut.CreateToken(NewUser());

        Assert.InRange(expiresAt, DateTime.UtcNow.AddMinutes(29), DateTime.UtcNow.AddMinutes(31));
    }

    [Fact]
    public void CreateToken_DefaultsToOneDay_WhenExpiryNotConfigured()
    {
        var sut = new TokenService(BuildConfig(expiresMinutes: null));

        var (_, expiresAt) = sut.CreateToken(NewUser());

        Assert.InRange(expiresAt, DateTime.UtcNow.AddMinutes(1439), DateTime.UtcNow.AddMinutes(1441));
    }

    [Fact]
    public void CreateToken_ReadsJwtSectionFromConfiguration()
    {
        // Mocked IConfiguration: verifies the service reads its settings from the "Jwt" section.
        var section = new Mock<IConfigurationSection>();
        section.Setup(s => s["Key"]).Returns(Key);
        section.Setup(s => s["Issuer"]).Returns("mock-issuer");
        section.Setup(s => s["Audience"]).Returns("mock-audience");
        section.Setup(s => s["ExpiresMinutes"]).Returns("5");
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c.GetSection("Jwt")).Returns(section.Object);
        var sut = new TokenService(configuration.Object);

        var (token, _) = sut.CreateToken(NewUser());

        Assert.Equal("mock-issuer", new JwtSecurityTokenHandler().ReadJwtToken(token).Issuer);
        configuration.Verify(c => c.GetSection("Jwt"), Times.Once);
    }
}
