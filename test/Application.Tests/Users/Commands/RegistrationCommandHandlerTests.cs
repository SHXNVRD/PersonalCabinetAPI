using Application.Services;
using Application.Users.Commands.Registration;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Users.Commands;

public class RegistrationCommandHandlerTests
{
    private readonly string _email = "test@mail.com";
    private readonly string _password = "password";
    private readonly string _phoneNumber = "88003553535";
    private readonly string _userName = "Test";
    private readonly string _role = "user";
    private readonly RegistrationCommand _command = new();
    
    private readonly Mock<AppUserManager> _appUserManagerMock = 
        new(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

    public RegistrationCommandHandlerTests()
    {
        _command.Email = _email;
        _command.Password = _password;
        _command.PhoneNumber = _phoneNumber;
        _command.UserName = _userName;
    }

    [Fact]
    public async Task Handle_FailedToCreateUser_ReturnsFail()
    {
        IdentityError[] identityErrors = [new IdentityError()];
        
        _appUserManagerMock
            .Setup(x => x.CreateAsync(
                    It.Is<User>(
                        u => u.UserName == _command.UserName && 
                             u.Email == _command.Email && 
                             u.PhoneNumber == _command.PhoneNumber),
                    _command.Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        var handler = new RegistrationCommandHandler(_appUserManagerMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsFailed);
        
        _appUserManagerMock.Verify(
            x => x.CreateAsync(It.Is<User>(
                u => u.UserName == _command.UserName && 
                     u.Email == _command.Email && 
                     u.PhoneNumber == _command.PhoneNumber),
                _command.Password), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_FailedToAddUserToRole_ReturnsFail()
    {
        IdentityError[] identityErrors = [new IdentityError()];
        
        _appUserManagerMock
            .Setup(x => x.CreateAsync(
                It.Is<User>(
                    u => u.UserName == _command.UserName && 
                         u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber),
                _command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _appUserManagerMock
            .Setup(x => x.AddToRoleAsync(
                It.Is<User>(
                    u => u.UserName == _command.UserName &&
                         u.Email == _command.Email &&
                         u.PhoneNumber == _command.PhoneNumber),
                _role))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        var handler = new RegistrationCommandHandler(_appUserManagerMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsFailed);
        
        _appUserManagerMock.Verify(
            x => x.CreateAsync(
                It.Is<User>(
                    u => u.UserName == _command.UserName && 
                         u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber),
                _command.Password), 
            Times.Once);
        
        _appUserManagerMock.Verify(
            x => x.AddToRoleAsync(
                It.Is<User>(
                    u => u.UserName == _command.UserName &&
                         u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber), 
                _role), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        _appUserManagerMock
            .Setup(x => x.CreateAsync(
                It.Is<User>(
                    u => u.UserName == _command.UserName && 
                         u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber),
                _command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _appUserManagerMock
            .Setup(x => x.AddToRoleAsync(
                It.Is<User>(
                    u => u.UserName == _command.UserName &&
                         u.Email == _command.Email &&
                         u.PhoneNumber == _command.PhoneNumber),
                _role))
            .ReturnsAsync(IdentityResult.Success);

        var handler = new RegistrationCommandHandler(_appUserManagerMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsSuccess);
        
        _appUserManagerMock.Verify(
            x => x.CreateAsync(
                It.Is<User>(
                    u => u.UserName == _command.UserName && 
                         u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber),
                _command.Password), 
            Times.Once);
        
        _appUserManagerMock.Verify(
            x => x.AddToRoleAsync(
                It.Is<User>(
                    u => u.UserName == _command.UserName &&
                         u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber), 
                _role), 
            Times.Once);
    }
}