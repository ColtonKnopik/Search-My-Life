using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SearchMyLife.Api.DTOs;
using SearchMyLife.Api.Services;
using SearchMyLife.Api.Tests.Helpers;

namespace SearchMyLife.Api.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly SearchMyLife.Api.Data.AppDbContext _db;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _db = TestDbContextFactory.Create();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "ThisIsATestKeyThatIsLongEnoughForHmacSha256!!",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:ExpiresInHours"] = "1"
            })
            .Build();

        _sut = new AuthService(_db, config);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task RegisterAsync_CreatesUserAndReturnsToken()
    {
        var request = new RegisterRequest { Email = "new@example.com", Password = "Password1!" };

        var result = await _sut.RegisterAsync(request);

        result.Token.Should().NotBeNullOrWhiteSpace();
        result.User.Email.Should().Be("new@example.com");
        result.User.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RegisterAsync_NormalisesEmailToLowerCase()
    {
        var request = new RegisterRequest { Email = "  Test@EXAMPLE.com  ", Password = "Password1!" };

        var result = await _sut.RegisterAsync(request);

        result.User.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task RegisterAsync_ThrowsInvalidOperationException_WhenDuplicateEmail()
    {
        var request = new RegisterRequest { Email = "dupe@example.com", Password = "Password1!" };
        await _sut.RegisterAsync(request);

        var act = () => _sut.RegisterAsync(new RegisterRequest { Email = "dupe@example.com", Password = "Other1!" });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
    {
        await _sut.RegisterAsync(new RegisterRequest { Email = "login@example.com", Password = "ValidPass1!" });

        var result = await _sut.LoginAsync(new LoginRequest { Email = "login@example.com", Password = "ValidPass1!" });

        result.Token.Should().NotBeNullOrWhiteSpace();
        result.User.Email.Should().Be("login@example.com");
    }

    [Fact]
    public async Task LoginAsync_ThrowsUnauthorizedAccessException_WhenPasswordIsWrong()
    {
        await _sut.RegisterAsync(new RegisterRequest { Email = "user@example.com", Password = "CorrectPass1!" });

        var act = () => _sut.LoginAsync(new LoginRequest { Email = "user@example.com", Password = "WrongPass1!" });

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Invalid*");
    }

    [Fact]
    public async Task LoginAsync_ThrowsUnauthorizedAccessException_WhenUserDoesNotExist()
    {
        var act = () => _sut.LoginAsync(new LoginRequest { Email = "ghost@example.com", Password = "Pass1!" });

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LoginAsync_IsCaseInsensitiveOnEmail()
    {
        await _sut.RegisterAsync(new RegisterRequest { Email = "case@example.com", Password = "Pass1!" });

        var result = await _sut.LoginAsync(new LoginRequest { Email = "  CASE@EXAMPLE.COM  ", Password = "Pass1!" });

        result.User.Email.Should().Be("case@example.com");
    }
}
