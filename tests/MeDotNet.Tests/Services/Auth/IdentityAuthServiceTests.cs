using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MeDotNet.Models;
using MeDotNet.Services.Auth;

namespace MeDotNet.Tests.Services.Auth;

public class IdentityAuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly IdentityAuthService _authService;

    public IdentityAuthServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            null, null, null, null);

        _authService = new IdentityAuthService(_userManagerMock.Object, _signInManagerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsSuccess_WhenIdentitySucceeds()
    {
        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _authService.RegisterAsync("test@example.com", "Password123!");

        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_ReturnsFailure_WhenIdentityFails()
    {
        var errors = new[] { new IdentityError { Description = "Email already taken." } };
        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(errors));

        var result = await _authService.RegisterAsync("test@example.com", "Password123!");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Email already taken.");
    }

    [Fact]
    public async Task SignInAsync_ReturnsSuccess_WhenCredentialsAreValid()
    {
        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var result = await _authService.SignInAsync("test@example.com", "Password123!");

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task SignInAsync_ReturnsFailure_WhenCredentialsAreInvalid()
    {
        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var result = await _authService.SignInAsync("test@example.com", "wrongpassword");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task SignOutAsync_CallsSignInManagerSignOut()
    {
        _signInManagerMock
            .Setup(x => x.SignOutAsync())
            .Returns(Task.CompletedTask);

        await _authService.SignOutAsync();

        _signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsUser_WhenPrincipalIsAuthenticated()
    {
        var user = new ApplicationUser { Id = "1", Email = "test@example.com" };
        _userManagerMock
            .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var result = await _authService.GetCurrentUserAsync(new ClaimsPrincipal());

        result.Should().Be(user);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsNull_WhenUserNotFound()
    {
        _userManagerMock
            .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.GetCurrentUserAsync(new ClaimsPrincipal());

        result.Should().BeNull();
    }
}
