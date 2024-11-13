using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Helpers;
using Application.Interfaces.Repositories;
using Domain.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Cards.Commands.Activate
{
    public class ActivateCardCommandHandler : IRequestHandler<ActivateCardCommand, Result<CardActivatedResponse>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICardRepository _cardRepository;

        public ActivateCardCommandHandler(UserManager<User> userManager, ICardRepository cardRepository)
        {
            _userManager = userManager;
            _cardRepository = cardRepository;
        }

        public async Task<Result<CardActivatedResponse>> Handle(ActivateCardCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
                return Result.Fail("User with specified id not found");

            var card = await _cardRepository.FindByNumberAsync(request.CardNumber);

            if (card == null)
                return Result.Fail($"Card with specified number not found");

            var codeHash = await Hasher.ComputeSha256HashAsync(request.CardCode);
            var activated =  await _cardRepository.ActivateAsync(user.Id, card.Id, codeHash);  

            if (!activated)
                return Result.Fail($"Failed to activated card");
            
            return Result.Ok(new CardActivatedResponse()
            {
                ActivatedCardId = card.Id
            });
        }
    }
}