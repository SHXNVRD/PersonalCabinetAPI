using System.Xml.XPath;
using Application.Cards.Commands.Activate;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Tests.Application;

public class ActivateCardCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly ActivateCardCommandHandler _handler;

    public ActivateCardCommandHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cardRepositoryMock = new Mock<ICardRepository>();

        _unitOfWorkMock.SetupGet(u => u.CardRepository).Returns(_cardRepositoryMock.Object);

        _handler = new ActivateCardCommandHandler(_userManagerMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCardActivatedSuccessfully()
    {
        // Arrange
        var command = new ActivateCardCommand
        {
            UserId = "123",
            CardNumber = 456,
            CardCode = "secret-code"
        };

        var user = new User { Id = 123 };

        _userManagerMock
            .Setup(u => u.FindByIdAsync(command.UserId))
            .ReturnsAsync(user);

        _cardRepositoryMock
            .Setup(r => r.ActivateAsync(user.Id, command.CardNumber, It.IsAny<string>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserNotFound()
    {
        // Arrange
        var command = new ActivateCardCommand
        {
            UserId = "123",
            CardNumber = 456,
            CardCode = "secret-code"
        };

        _userManagerMock.Setup(u => u.FindByIdAsync(command.UserId))
            .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenChangesNotSaved()
    {
        // Arrange
        var command = new ActivateCardCommand
        {
            UserId = "123",
            CardNumber = 456,
            CardCode = "secret-code"
        };

        var user = new User { Id = 123 };

        _userManagerMock.Setup(u => u.FindByIdAsync(command.UserId))
            .ReturnsAsync(user);

        _cardRepositoryMock.Setup(r => r.ActivateAsync(user.Id, command.CardNumber, It.IsAny<string>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }
}