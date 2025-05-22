using Application.Interfaces;
using Application.Services;
using Domain.Aggregates.CardAggregate;
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
    public BlockCardCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(BlockCardCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.FindByIdAsync(request.UserId, TrackingType.Tracking);
        if (user == null)
            return Result.Fail(new NotFoundError($"User with id {request.UserId} was not found"));

        var card = user.Cards.SingleOrDefault(c => c.Id == request.CardId);
        if (card == null)
            return Result.Fail(new NotFoundError($"Card with id {request.CardId} was not found"));
        
        var blockResult = card.Block();
        if (blockResult.IsFailed)
            return Result.Fail(blockResult.Errors);

        _unitOfWork.CardRepository.UpdateStatus(card);
        var changesSaved = await _unitOfWork.SaveChangesAsync();

        return Result.OkIf(changesSaved, "Failed to save changes");
    }
}