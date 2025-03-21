using System.Security.Policy;
using Application.DTOs;
using Application.DTOs.Emails;
using Application.Extensions;
using Application.Interfaces;
using Application.Interfaces.Email;
using Application.Interfaces.Token;
using Application.Services;
using Application.Users.DTOs;
using Domain.Aggregates.UserAggregate;
using Domain.Shared.Errors;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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
        var createUserResult = User.Create(request.Email, request.PhoneNumber, request.UserName);
        if (createUserResult.IsFailed)
            return Result.Fail(createUserResult.Errors);

        var user = createUserResult.Value;
        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var userResult = await _userManager.CreateAsync(user, request.Password);
            if (!userResult.Succeeded)
                return Result.Fail(new Conflict(userResult.Errors.First().Description));
                
            var roleResult = await _userManager.AddToRoleAsync(user, "user");
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail(new Conflict(roleResult.Errors.First().Description));
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