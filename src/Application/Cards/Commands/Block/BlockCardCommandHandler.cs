using Application.Interfaces;
using Application.Services;
using Domain.Aggregates.UserAggregate;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Cards.Commands.Block;

public class BlockCardCommandHandler : IRequestHandler<BlockCardCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppUserManager _userManager; 

    public BlockCardCommandHandler(IUnitOfWork unitOfWork, AppUserManager userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<Result> Handle(BlockCardCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return Result.Fail(new NotFound($"User with id {request.UserId} was not found"));

        var cardNumberResult = CardNumber.Create(request.Number);
        if (cardNumberResult.IsFailed)
            return Result.Fail(cardNumberResult.Errors);

        var blockResult = user.BlockCard(cardNumberResult.Value);
        if (blockResult.IsFailed)
            return Result.Fail(blockResult.Errors);

        var changesSaved = await _unitOfWork.SaveChangesAsync();

        return Result.OkIf(changesSaved, "Failed to save changes");
    }
}