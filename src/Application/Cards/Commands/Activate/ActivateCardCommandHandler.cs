using Application.Interfaces;
using Application.Services;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.Cards.Commands.Activate;

public class ActivateCardCommandHandler : IRequestHandler<ActivateCardCommand, Result<ActivateCardResponse>>
{
    private readonly AppUserManager _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateCardCommandHandler(AppUserManager userManager, IUnitOfWork unitOfWork)
    {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
    }

    public async Task<Result<ActivateCardResponse>> Handle(ActivateCardCommand request, CancellationToken cancellationToken)
    {
            var cardNumberResult = CardNumber.Create(request.CardNumber);
            if (cardNumberResult.IsFailed)
                return Result.Fail(cardNumberResult.Errors);

            var card = await _unitOfWork.CardRepository.FindByNumberAsync(cardNumberResult.Value, TrackingType.Tracking);
            if (card == null)
                return Result.Fail(new NotFound($"Card with number {request.CardNumber} was not found"));

            if (!Guid.TryParse(request.UserId, out var userId))
                return Result.Fail(new InvalidData("Invalid user id"));
            
            var activateResult = card.Activate(userId);
            if (activateResult.IsFailed)
                return Result.Fail(activateResult.Errors);
            
            var pinHashResult = CardPinHash.Create(request.CardPin);
            if (pinHashResult.IsFailed)
                return Result.Fail(pinHashResult.Errors);

            var changePinResult = card.ChangePin(pinHashResult.Value);
            if (changePinResult.IsFailed)
                return Result.Fail(changePinResult.Errors);

            _unitOfWork.CardRepository.UpdateStatus(card);
            var changesSaved = await _unitOfWork.SaveChangesAsync();
            if (!changesSaved)
                return Result.Fail("Failed to save changes");

            return Result.Ok(new ActivateCardResponse(card.Id));
    }
}