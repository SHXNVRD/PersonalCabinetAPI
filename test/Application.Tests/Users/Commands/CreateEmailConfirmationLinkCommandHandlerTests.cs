using Application.DTOs.Emails;
using Application.Interfaces;
using Application.Interfaces.Email;
using Application.Services;
using Application.Users.Commands.CreateEmailConfirmationLink;
using Domain.Aggregates.UserAggregate;
using Moq;
using Microsoft.AspNetCore.Identity;

namespace Application.Tests.Users.Commands;

public class CreateEmailConfirmationLinkCommandHandlerTests
{
    private readonly string _email = "test@mail.com";
    private readonly string _phoneNumber = "1234567890";
    private readonly string _name = "Test";
    private readonly string _token = "token";
    private readonly string _confirmationLink = "link";
    private readonly CreateEmailConfirmationLinkCommand _command = new();

    private readonly Mock<ILinkService> _linkServiceMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<AppUserManager> _appUserManagerMock =
        new(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
    
    public CreateEmailConfirmationLinkCommandHandlerTests()
    {
        _command.Email = _email;
    }
    
    [Fact]
    public async Task Handle_NonExistEmail_ReturnsFail()
    {
        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(default(User));
        
        var handler = new CreateEmailConfirmationLinkCommandHandler(
            _emailServiceMock.Object, 
            _linkServiceMock.Object,
            _appUserManagerMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public async Task Handle_ValidEmail_ReturnsSuccess()
    {
        var user = User.Create(_email, _phoneNumber, _name).Value;
        
        _appUserManagerMock
            .Setup(x => x.FindByEmailAsync(_command.Email))
            .ReturnsAsync(user);
        _appUserManagerMock
            .Setup(x => x.GenerateEmailConfirmationTokenAsync(user))
            .ReturnsAsync(_token);
        _linkServiceMock
            .Setup(x => x.GetUriByAction(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<string>()))
            .Returns(_confirmationLink);
        _emailServiceMock
            .Setup(x => x.SendEmailConfirmationLinkAsync(
                It.Is<EmailMessage>(m => m.To.Contains(_command.Email)),
                _confirmationLink,
                default))
            .ReturnsAsync(true);
        
        var handler = new CreateEmailConfirmationLinkCommandHandler(
            _emailServiceMock.Object, 
            _linkServiceMock.Object,
            _appUserManagerMock.Object);

        var result = await handler.Handle(_command, default);
        
        Assert.True(result.IsSuccess);
        
        _emailServiceMock.Verify(
            x => x.SendEmailConfirmationLinkAsync(
                It.Is<EmailMessage>(m => m.To.Contains(_command.Email)),
                _confirmationLink,
                default),
            Times.Once());
    }
}
