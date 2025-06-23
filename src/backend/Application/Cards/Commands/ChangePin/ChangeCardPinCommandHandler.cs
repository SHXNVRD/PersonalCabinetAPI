using Application.Interfaces;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.Cards.Commands.ChangePin;

public class ChangeCardPinCommandHandler : IRequestHandler<ChangeCardPinCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ChangeCardPinCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeCardPinCommand request, CancellationToken cancellationToken)
    {
        var card = await _unitOfWork.CardRepository.FindByIdAsync(request.CardId, TrackingType.Tracking);
        if (card is null)
            return Result.Fail(Errors.Conflict.NotFound($"Card with id {request.CardId} was not found"));

        var pinHashResult = CardPinHash.Create(request.Pin);
        if (pinHashResult.IsFailed)
            return Result.Fail(pinHashResult.Errors);

        var changeResult = card.ChangePin(pinHashResult.Value);
        if (changeResult.IsFailed)
            return Result.Fail(changeResult.Errors);
        
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}