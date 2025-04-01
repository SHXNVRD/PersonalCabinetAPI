using Application.Services;
using Application.Users.Commands.EmailConfirmation;
using Domain.Aggregates.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Users.Commands;

public class EmailConfirmationCommandHandlerTests
{
    private readonly string _email = "test@mail.com";
    private readonly string _phoneNumber = "1234567890";
    private readonly string _name = "Test";
    private readonly string _token = "token";
    private readonly EmailConfirmationCommand _command = new ();
    
    private readonly Mock<AppUserManager> _appUserManagerMock = 
        new(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

    public EmailConfirmationCommandHandlerTests()
    {
        _command.Email = _email;
        _command.Token = _token;
    }
    
    [Fact]
    public async Task Handle_NonExistEmail_ReturnsFail()
    {
        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(default(User));

        var handler = new EmailConfirmationCommandHandler(_appUserManagerMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_FailedToConfirmEmail_ReturnsFail()
    {
        var user = User.Create(_email, _phoneNumber, _name).Value;
        var error = new IdentityError
        {
            Code = "Code",
            Description = "Description"
        };
        var identityResult = IdentityResult.Failed(error);
            
        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);
        _appUserManagerMock
            .Setup(x => x.ConfirmEmailAsync(user, _command.Token))
            .ReturnsAsync(identityResult);

        var handler = new EmailConfirmationCommandHandler(_appUserManagerMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsFailed);
        
        _appUserManagerMock.Verify(
            x => x.ConfirmEmailAsync(It.Is<User>(u => u.Email == _command.Email), _token),
            Times.Once());
    }

    [Fact]
    public async Task Handle_ValidEmail_ReturnsSuccess()
    {
        var user = User.Create(_email, _phoneNumber, _name).Value;
        
        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);
        _appUserManagerMock
            .Setup(x => x.ConfirmEmailAsync(user, _command.Token))
            .ReturnsAsync(IdentityResult.Success);

        var handler = new EmailConfirmationCommandHandler(_appUserManagerMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsSuccess);
        
        _appUserManagerMock.Verify(
            x => x.ConfirmEmailAsync(It.Is<User>(u => u.Email == _command.Email), _token),
            Times.Once());
    }
}