using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SearchMyLife.Api.Controllers;
using SearchMyLife.Api.DTOs;
using SearchMyLife.Api.Services;

namespace SearchMyLife.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly ILogger<AuthController> _logger = Substitute.For<ILogger<AuthController>>();
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _sut = new AuthController(_authService, _logger);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenSuccessful()
    {
        var request = new RegisterRequest { Email = "a@b.com", Password = "Pass1234!" };
        var response = new AuthResponse
        {
            Token = "jwt-token",
            User = new UserDto { Id = Guid.NewGuid(), Email = "a@b.com" }
        };
        _authService.RegisterAsync(request).Returns(response);

        var result = await _sut.Register(request);

        result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result).Value.Should().Be(response);
    }

    [Fact]
    public async Task Register_ReturnsConflict_WhenDuplicateEmail()
    {
        var request = new RegisterRequest { Email = "dupe@b.com", Password = "Pass1234!" };
        _authService.RegisterAsync(request).Throws(new InvalidOperationException("already exists"));

        var result = await _sut.Register(request);

        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Register_Returns500_OnUnexpectedError()
    {
        var request = new RegisterRequest { Email = "a@b.com", Password = "Pass1234!" };
        _authService.RegisterAsync(request).Throws(new Exception("boom"));

        var result = await _sut.Register(request);

        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsAreValid()
    {
        var request = new LoginRequest { Email = "a@b.com", Password = "Pass1234!" };
        var response = new AuthResponse
        {
            Token = "jwt-token",
            User = new UserDto { Id = Guid.NewGuid(), Email = "a@b.com" }
        };
        _authService.LoginAsync(request).Returns(response);

        var result = await _sut.Login(request);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        var request = new LoginRequest { Email = "a@b.com", Password = "wrong" };
        _authService.LoginAsync(request).Throws(new UnauthorizedAccessException("Invalid"));

        var result = await _sut.Login(request);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_Returns500_OnUnexpectedError()
    {
        var request = new LoginRequest { Email = "a@b.com", Password = "Pass1234!" };
        _authService.LoginAsync(request).Throws(new Exception("boom"));

        var result = await _sut.Login(request);

        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
    }
}
