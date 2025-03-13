using Application.Interfaces.Token;
using Application.Services;
using Application.Users.Commands.Login;
using Domain.Aggregates.UserAggregate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Application.Tests.Users.Commands;

public class LoginCommandHandlerTests
{
    private readonly string _email = "test@mail.com";
    private readonly string _password = "password";
    private readonly LoginCommand _command = new();
    
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<AppUserManager> _appUserManagerMock = 
        new(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

    public LoginCommandHandlerTests()
    {
        _command.Email = _email;
        _command.Password = _password;
        _signInManagerMock = new Mock<SignInManager<User>>(
            _appUserManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<User>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object);
    }       

    [Fact]
    public async Task Handle_UserNotFoundWithSpecifiedEmail_ReturnsFail()
    {
        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(default(User));

        var handler = new LoginCommandHandler(
            _signInManagerMock.Object,
            _appUserManagerMock.Object,
            _tokenServiceMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_UnconfirmedEmail_ReturnsFail()
    {
        User user = new()
        {
            EmailConfirmed = false
        };
        
        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);

        var handler = new LoginCommandHandler(
            _signInManagerMock.Object,
            _appUserManagerMock.Object,
            _tokenServiceMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsFail()
    {
        User user = new()
        {
            Email = _command.Email,
            EmailConfirmed = true
        };
        
        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);
        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, _command.Password, false))
            .ReturnsAsync(SignInResult.Failed);
        
        var handler = new LoginCommandHandler(
            _signInManagerMock.Object,
            _appUserManagerMock.Object,
            _tokenServiceMock.Object);

        var result = await handler.Handle(_command, default);

        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        User user = new()
        {
            Email = _command.Email,
            EmailConfirmed = true
        };
        var accessToken = "accessToken";
        var refreshToken = "refreshToken";
        var tokenType = "tokenType";
        var accessTokenExpiresInSeconds = 1;
        
        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);
        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, _command.Password, false))
            .ReturnsAsync(SignInResult.Success);
        _tokenServiceMock
            .Setup(x => x.GenerateTokenAsync(user))
            .ReturnsAsync(accessToken);
        _tokenServiceMock
            .Setup(x => x.GenerateRefreshTokenAsync(user))
            .ReturnsAsync(refreshToken);
        _tokenServiceMock
            .SetupGet(x => x.TokenType)
            .Returns(tokenType);
        _tokenServiceMock
            .SetupGet(x => x.AccessTokenExpiresInSeconds)
            .Returns(accessTokenExpiresInSeconds);
        
        var handler = new LoginCommandHandler(
            _signInManagerMock.Object,
            _appUserManagerMock.Object,
            _tokenServiceMock.Object);

        var result = await handler.Handle(_command, default);

        Assert.True(result.IsSuccess);
        
        var resultValue = result.Value;
        
        Assert.Equal(accessToken, resultValue.Token);
        Assert.Equal(refreshToken, resultValue.RefreshToken);
        Assert.Equal(tokenType, resultValue.TokenType);
        Assert.Equal(accessTokenExpiresInSeconds, resultValue.ExpiresIn);
    }
}