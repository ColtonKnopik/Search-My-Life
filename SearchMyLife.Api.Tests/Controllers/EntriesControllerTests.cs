using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SearchMyLife.Api.Controllers;
using SearchMyLife.Api.DTOs;
using SearchMyLife.Api.Services;

namespace SearchMyLife.Api.Tests.Controllers;

public class EntriesControllerTests
{
    private readonly IJournalService _journalService = Substitute.For<IJournalService>();
    private readonly IVectorSearchService _vectorSearchService = Substitute.For<IVectorSearchService>();
    private readonly EntriesController _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public EntriesControllerTests()
    {
        _sut = new EntriesController(_journalService, _vectorSearchService);
        SetUser(_sut, _userId);
    }

    private static void SetUser(ControllerBase controller, Guid userId)
    {
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithEntries()
    {
        var entries = new List<EntryResponse>
        {
            new() { Id = Guid.NewGuid(), Title = "A", Content = "c" }
        };
        _journalService.GetAllAsync(_userId).Returns(entries);

        var result = await _sut.GetAll();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(entries);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNull()
    {
        _journalService.GetByIdAsync(Arg.Any<Guid>(), _userId).Returns((EntryResponse?)null);

        var result = await _sut.GetById(Guid.NewGuid());

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var entryId = Guid.NewGuid();
        var entry = new EntryResponse { Id = entryId, Title = "Found", Content = "c" };
        _journalService.GetByIdAsync(entryId, _userId).Returns(entry);

        var result = await _sut.GetById(entryId);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(entry);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var request = new CreateEntryRequest { Title = "New", Content = "c" };
        var created = new EntryResponse { Id = Guid.NewGuid(), Title = "New", Content = "c" };
        _journalService.CreateAsync(_userId, request).Returns(created);

        var result = await _sut.Create(request);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.Value.Should().Be(created);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenNull()
    {
        var request = new UpdateEntryRequest { Title = "U", Content = "c" };
        _journalService.UpdateAsync(Arg.Any<Guid>(), _userId, request).Returns((EntryResponse?)null);

        var result = await _sut.Update(Guid.NewGuid(), request);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenFound()
    {
        var entryId = Guid.NewGuid();
        var request = new UpdateEntryRequest { Title = "U", Content = "c" };
        var updated = new EntryResponse { Id = entryId, Title = "U", Content = "c" };
        _journalService.UpdateAsync(entryId, _userId, request).Returns(updated);

        var result = await _sut.Update(entryId, request);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenFalse()
    {
        _journalService.DeleteAsync(Arg.Any<Guid>(), _userId).Returns(false);

        var result = await _sut.Delete(Guid.NewGuid());

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_AndRemovesEmbedding()
    {
        var entryId = Guid.NewGuid();
        _journalService.DeleteAsync(entryId, _userId).Returns(true);

        var result = await _sut.Delete(entryId);

        result.Should().BeOfType<NoContentResult>();
        await _vectorSearchService.Received(1).DeleteEmbeddingAsync(entryId);
    }
}
