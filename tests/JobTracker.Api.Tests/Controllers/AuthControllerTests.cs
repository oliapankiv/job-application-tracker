using JobTracker.Api.Controllers;
using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using JobTracker.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace JobTracker.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly AuthController _sut;
    private readonly DateTime _expiresAt = new(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    public AuthControllerTests()
    {
        _userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);

        _tokenService
            .Setup(t => t.CreateToken(It.IsAny<ApplicationUser>()))
            .Returns(("jwt-token", _expiresAt));

        _sut = new AuthController(_userManager.Object, _tokenService.Object);
    }

    [Fact]
    public async Task Register_ReturnsConflict_WhenEmailAlreadyExists()
    {
        _userManager.Setup(m => m.FindByEmailAsync("jane@acme.test")).ReturnsAsync(new ApplicationUser());

        var result = await _sut.Register(new RegisterDto("jane@acme.test", "password123", "Jane"));

        Assert.IsType<ConflictObjectResult>(result.Result);
        _userManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        _tokenService.Verify(t => t.CreateToken(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenIdentityRejectsUser()
    {
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        _userManager
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        var result = await _sut.Register(new RegisterDto("jane@acme.test", "password123", null));

        Assert.IsType<BadRequestObjectResult>(result.Result);
        _tokenService.Verify(t => t.CreateToken(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Register_CreatesUserAndReturnsToken()
    {
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        _userManager
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), "password123"))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _sut.Register(new RegisterDto("jane@acme.test", "password123", "Jane"));

        var response = Assert.IsType<AuthResponseDto>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(new AuthResponseDto("jwt-token", _expiresAt, "jane@acme.test", "Jane"), response);
        _userManager.Verify(m => m.CreateAsync(
            It.Is<ApplicationUser>(u => u.Email == "jane@acme.test" && u.UserName == "jane@acme.test" && u.DisplayName == "Jane"),
            "password123"), Times.Once);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
    {
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

        var result = await _sut.Login(new LoginDto("nobody@acme.test", "whatever"));

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        _userManager.Verify(m => m.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenPasswordInvalid()
    {
        var user = new ApplicationUser { Email = "jane@acme.test" };
        _userManager.Setup(m => m.FindByEmailAsync("jane@acme.test")).ReturnsAsync(user);
        _userManager.Setup(m => m.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);

        var result = await _sut.Login(new LoginDto("jane@acme.test", "wrong"));

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        _tokenService.Verify(t => t.CreateToken(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenCredentialsValid()
    {
        var user = new ApplicationUser { Email = "jane@acme.test", DisplayName = "Jane" };
        _userManager.Setup(m => m.FindByEmailAsync("jane@acme.test")).ReturnsAsync(user);
        _userManager.Setup(m => m.CheckPasswordAsync(user, "password123")).ReturnsAsync(true);

        var result = await _sut.Login(new LoginDto("jane@acme.test", "password123"));

        var response = Assert.IsType<AuthResponseDto>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal("jwt-token", response.Token);
        Assert.Equal("Jane", response.DisplayName);
        _tokenService.Verify(t => t.CreateToken(user), Times.Once);
    }
}
