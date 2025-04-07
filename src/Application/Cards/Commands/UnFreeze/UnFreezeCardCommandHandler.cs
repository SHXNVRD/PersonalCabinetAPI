using Application.Interfaces;
using Application.Services;
using Domain.Shared.Errors;
using FluentResults;
using MediatR;

namespace Application.Cards.Commands.UnFreeze;

public class UnFreezeCardCommandHandler : IRequestHandler<UnFreezeCardCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppUserManager _userManager;

    public async Task<Result> Handle(UnFreezeCardCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return Result.Fail(new NotFound($"User with id {request.UserId} was not found"));

        var blockResult = user.UnFreezeCard(request.CardId);
        if (blockResult.IsFailed)
            return Result.Fail(blockResult.Errors);

        var changesSaved = await _unitOfWork.SaveChangesAsync();

        return Result.OkIf(changesSaved, "Failed to save changes");
    }
}