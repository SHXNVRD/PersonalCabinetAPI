using Application.Interfaces;
using Domain.Shared.Errors;
using FluentResults;
using MediatR;

namespace Application.Cards.Commands.UnFreeze;

public class UnFreezeCardCommandHandler : IRequestHandler<UnFreezeCardCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnFreezeCardCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnFreezeCardCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.FindByIdAsync(request.UserId, TrackingType.Tracking);
        if (user == null)
            return Result.Fail(Errors.Conflict.NotFound($"User with id {request.UserId} was not found"));

        var card = user.Cards.SingleOrDefault(c => c.Id == request.CardId);
        if (card == null)
            return Result.Fail(Errors.Conflict.NotFound($"Card with id {request.CardId} was not found"));
        
        var unFreezeResult = card.UnFreeze();
        if (unFreezeResult.IsFailed)
            return Result.Fail(unFreezeResult.Errors);

        _unitOfWork.CardRepository.UpdateStatus(card);
        var changesSaved = await _unitOfWork.SaveChangesAsync();
        
        return Result.OkIf(changesSaved, "Failed to save changes");
    }
}