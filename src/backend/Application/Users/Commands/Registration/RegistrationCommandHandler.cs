using Application.Extensions;
using Application.Interfaces;
using Application.Services;
using Domain.Aggregates.UserAggregate;
using Domain.Shared.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.Registration;

public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, Result>
{
    private readonly AppUserManager _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrationCommandHandler(AppUserManager userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        var userName = UserNameGenerator.GenerateByEmail(request.Email);
        var nameResult = Name.Create(request.FirstName, request.LastName, request.Patronymic);
        if (nameResult.IsFailed)
            return Result.Fail(nameResult.Errors);
        
        var createUserResult = User.Create(request.Email, request.PhoneNumber, userName, nameResult.Value);
        if (createUserResult.IsFailed)
            return Result.Fail(createUserResult.Errors);

        var user = createUserResult.Value;
        
        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var userResult = await _userManager.CreateAsync(user, request.Password);
            if (!userResult.Succeeded)
                return userResult.ToFluentResult();
                
            var roleResult = await _userManager.AddToRoleAsync(user, Role.User);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return roleResult.ToFluentResult();
            }
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        await transaction.CommitAsync(cancellationToken);
        return Result.Ok();
    }
}