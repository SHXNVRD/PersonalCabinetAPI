using Application.Interfaces.Token;
using Application.Services;
using Application.Users.Commands.Login;
using Domain.Aggregates.UserAggregate;
using Domain.Shared.ValueObjects;
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
    private readonly string _userName = "test";
    private readonly Name _name = Name.Create("firstname", "lastname", "patronymic").Value;
    private readonly string _phoneNumber = "1234567890";
    private readonly string _password = "password";
    private readonly LoginCommand _commandWithEmail = new();
    private readonly LoginCommand _commandWithUserName = new();
    
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<AppUserManager> _appUserManagerMock = 
        new(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

    public LoginCommandHandlerTests()
    {
        _commandWithEmail.Login = _email;
        _commandWithEmail.Password = _password;

        _commandWithUserName.Login = _userName;
        _commandWithEmail.Password = _password;
        
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
            .Setup(x => x.FindByEmailAsync(_commandWithEmail.Login))
            .ReturnsAsync(default(User));

        var handler = new LoginCommandHandler(
            _signInManagerMock.Object,
            _appUserManagerMock.Object,
            _tokenServiceMock.Object);

        var result = await handler.Handle(_commandWithEmail, default);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_UnconfirmedEmail_ReturnsFail()
    {
        var user = User.Create(_email, _phoneNumber, _userName, _name).Value;

        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_commandWithEmail.Login))
            .ReturnsAsync(user);

        var handler = new LoginCommandHandler(
            _signInManagerMock.Object,
            _appUserManagerMock.Object,
            _tokenServiceMock.Object);

        var result = await handler.Handle(_commandWithEmail, default);
        
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public async Task Handle_UserNotFoundWithSpecifiedUserName_ReturnsFail()
    {
        _appUserManagerMock
            .Setup(x => x.FindByNameAsync(_commandWithEmail.Login))
            .ReturnsAsync(default(User));

        var handler = new LoginCommandHandler(
            _signInManagerMock.Object,
            _appUserManagerMock.Object,
            _tokenServiceMock.Object);

        var result = await handler.Handle(_commandWithUserName, default);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsFail()
    {
        var user = User.Create(_email, _phoneNumber, _userName, _name).Value;

        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_commandWithEmail.Login))
            .ReturnsAsync(user);
        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, _commandWithEmail.Password, false))
            .ReturnsAsync(SignInResult.Failed);
        
        var handler = new LoginCommandHandler(
            _signInManagerMock.Object,
            _appUserManagerMock.Object,
            _tokenServiceMock.Object);

        var result = await handler.Handle(_commandWithEmail, default);

        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public async Task Handle_SuccessCaseWithEmail_ReturnsSuccess()
    {
        var user = User.Create(_email, _phoneNumber, _userName, _name).Value;
        user.EmailConfirmed = true;

        var accessToken = "accessToken";
        var refreshToken = "refreshToken";
        var tokenType = "tokenType";
        var accessTokenExpiresInSeconds = 1;
        
        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_commandWithEmail.Login))
            .ReturnsAsync(user);
        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, _commandWithEmail.Password, false))
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

        var result = await handler.Handle(_commandWithEmail, default);

        Assert.True(result.IsSuccess);
        
        var resultValue = result.Value;
        
        Assert.Equal(accessToken, resultValue.AccessToken);
        Assert.Equal(refreshToken, resultValue.RefreshToken);
        Assert.Equal(tokenType, resultValue.TokenType);
        Assert.Equal(accessTokenExpiresInSeconds, resultValue.ExpiresIn);
    }
    
    [Fact]
    public async Task Handle_SuccessCaseWithUserName_ReturnsSuccess()
    {
        var user = User.Create(_email, _phoneNumber, _userName, _name).Value;
        user.EmailConfirmed = true;

        var accessToken = "accessToken";
        var refreshToken = "refreshToken";
        var tokenType = "tokenType";
        var accessTokenExpiresInSeconds = 1;
        
        _appUserManagerMock
            .Setup(x => x.FindByNameAsync(_commandWithUserName.Login))
            .ReturnsAsync(user);
        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, _commandWithUserName.Password, false))
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

        var result = await handler.Handle(_commandWithUserName, default);

        Assert.True(result.IsSuccess);
        
        var resultValue = result.Value;
        
        Assert.Equal(accessToken, resultValue.AccessToken);
        Assert.Equal(refreshToken, resultValue.RefreshToken);
        Assert.Equal(tokenType, resultValue.TokenType);
        Assert.Equal(accessTokenExpiresInSeconds, resultValue.ExpiresIn);
    }
}