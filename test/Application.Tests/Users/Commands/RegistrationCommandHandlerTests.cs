using System.Data;
using Application.Interfaces;
using Application.Services;
using Application.Users.Commands.Registration;
using Domain.Aggregates.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Application.Tests.Users.Commands;

public class RegistrationCommandHandlerTests
{
    private readonly string _email = "test@mail.com";
    private readonly string _password = "password";
    private readonly string _phoneNumber = "88003553535";
    private readonly string _firstName = "firstname";
    private readonly string _lastName = "lastname";
    private readonly string _patronymic = "patronymic";
    private readonly string _role = "user";
    private readonly RegistrationCommand _command = new();
    private readonly IdentityErrorDescriber _errorDescriber = new();
    
    private readonly Mock<AppUserManager> _appUserManagerMock = 
        new(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
    
    private readonly Mock<IDbContextTransaction> _transactionMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public RegistrationCommandHandlerTests()
    {
        _command.Email = _email;
        _command.Password = _password;
        _command.PhoneNumber = _phoneNumber;
        _command.FirstName = _firstName;
        _command.LastName = _lastName;
        _command.Patronymic = _patronymic;
    }

    [Fact]
    public async Task Handle_FailedToCreateUser_ReturnsFail()
    {
        var error = _errorDescriber.InvalidEmail(_command.Email);
        var failedIdentityResult = IdentityResult.Failed(error);
        
        _appUserManagerMock
            .Setup(x => x.CreateAsync(
                    It.Is<User>(
                        u => u.Email == _command.Email && 
                             u.PhoneNumber == _command.PhoneNumber &&
                             u.Name.FirstName == _command.FirstName &&
                             u.Name.LastName == _command.LastName &&
                             u.Name.Patronymic == _command.Patronymic),
                    _command.Password))
            .ReturnsAsync(failedIdentityResult);

        _unitOfWorkMock
            .Setup(x => x.BeginTransactionAsync(It.IsAny<bool>()))
            .ReturnsAsync(It.IsAny<IDbContextTransaction>());

        var handler = new RegistrationCommandHandler(_appUserManagerMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsFailed);
        
        _appUserManagerMock.Verify(
            x => x.CreateAsync(It.Is<User>(
                    u => u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber &&
                         u.Name.FirstName == _command.FirstName &&
                         u.Name.LastName == _command.LastName &&
                         u.Name.Patronymic == _command.Patronymic),
                _command.Password), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_FailedToAddUserToRole_ReturnsFail()
    {
        _appUserManagerMock
            .Setup(x => x.CreateAsync(
                It.Is<User>(
                    u => u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber &&
                         u.Name.FirstName == _command.FirstName &&
                         u.Name.LastName == _command.LastName &&
                         u.Name.Patronymic == _command.Patronymic),
                _command.Password))
            .ReturnsAsync(IdentityResult.Success);

        var error = _errorDescriber.UserAlreadyInRole(_role);
        var failedIdentityResult = IdentityResult.Failed(error);
        
        _appUserManagerMock
            .Setup(x => x.AddToRoleAsync(
                It.Is<User>(
                    u => u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber &&
                         u.Name.FirstName == _command.FirstName &&
                         u.Name.LastName == _command.LastName &&
                         u.Name.Patronymic == _command.Patronymic),
                _role))
            .ReturnsAsync(failedIdentityResult);
        
        _unitOfWorkMock
            .Setup(x => x.BeginTransactionAsync(It.IsAny<bool>()))
            .ReturnsAsync(_transactionMock.Object);

        var handler = new RegistrationCommandHandler(_appUserManagerMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsFailed);
        
        _appUserManagerMock.Verify(
            x => x.CreateAsync(
                It.Is<User>(
                    u => u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber &&
                         u.Name.FirstName == _command.FirstName &&
                         u.Name.LastName == _command.LastName &&
                         u.Name.Patronymic == _command.Patronymic),
                _command.Password), 
            Times.Once);
        
        _appUserManagerMock.Verify(
            x => x.AddToRoleAsync(
                It.Is<User>(
                    u => u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber &&
                         u.Name.FirstName == _command.FirstName &&
                         u.Name.LastName == _command.LastName &&
                         u.Name.Patronymic == _command.Patronymic),
                _role), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        _appUserManagerMock
            .Setup(x => x.CreateAsync(
                It.Is<User>(
                    u => u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber &&
                         u.Name.FirstName == _command.FirstName &&
                         u.Name.LastName == _command.LastName &&
                         u.Name.Patronymic == _command.Patronymic),
                _command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _appUserManagerMock
            .Setup(x => x.AddToRoleAsync(
                It.Is<User>(
                    u => u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber &&
                         u.Name.FirstName == _command.FirstName &&
                         u.Name.LastName == _command.LastName &&
                         u.Name.Patronymic == _command.Patronymic),
                _role))
            .ReturnsAsync(IdentityResult.Success);
        
        _unitOfWorkMock
            .Setup(x => x.BeginTransactionAsync(It.IsAny<bool>()))
            .ReturnsAsync(_transactionMock.Object);

        var handler = new RegistrationCommandHandler(_appUserManagerMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsSuccess);
        
        _appUserManagerMock.Verify(
            x => x.CreateAsync(
                It.Is<User>(
                    u => u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber &&
                         u.Name.FirstName == _command.FirstName &&
                         u.Name.LastName == _command.LastName &&
                         u.Name.Patronymic == _command.Patronymic),
                _command.Password), 
            Times.Once);
        
        _appUserManagerMock.Verify(
            x => x.AddToRoleAsync(
                It.Is<User>(
                    u => u.Email == _command.Email && 
                         u.PhoneNumber == _command.PhoneNumber &&
                         u.Name.FirstName == _command.FirstName &&
                         u.Name.LastName == _command.LastName &&
                         u.Name.Patronymic == _command.Patronymic),
                _role), 
            Times.Once);
    }
}