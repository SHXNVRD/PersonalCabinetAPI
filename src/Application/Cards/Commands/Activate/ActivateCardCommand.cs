using FluentResults;
using MediatR;

namespace Application.Cards.Commands.Activate
{
    public class ActivateCardCommand : IRequest<Result<ActivateCardResponse>>
    {
        public string UserId { get; set; }
        public string CardNumber { get; set; }
        public string CardPin { get; set; }
    }
}