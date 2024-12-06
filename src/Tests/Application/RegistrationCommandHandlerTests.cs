using Application.Services;
using Application.Users.Commands.Registration;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.Application;

public class RegistrationCommandHandlerTests
{
    private readonly Mock<AppUserManager> _userManagerMock;
    private readonly RegistrationCommandHandler _handler;

    public RegistrationCommandHandlerTests()
    {
        _userManagerMock = new Mock<AppUserManager>(
            new Mock<IUserStore<User>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<User>>().Object,
            new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object },
            new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object },
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<User>>>().Object
        );
        
        _handler = new RegistrationCommandHandler(_userManagerMock.Object);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_CreatesUserWithUserRole()
    {
        // Arrange
        RegistrationCommand command = new("Ivan", "1234567890", "ivan@mail.com", "P@ssword228");
        var userRole = "user";

        _userManagerMock
            .Setup(um => um.CreateAsync(It.Is<User>(u => 
                    u.UserName == command.UserName && 
                    u.Email == command.Email && 
                    u.PhoneNumber == command.PhoneNumber), 
                command.Password))
            .ReturnsAsync(IdentityResult.Success);
        
        _userManagerMock
            .Setup(um => um.AddToRoleAsync(It.Is<User>(u => 
                u.UserName == command.UserName), userRole))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), command.Password), Times.Once);
        _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), userRole), Times.Once);
    }
}