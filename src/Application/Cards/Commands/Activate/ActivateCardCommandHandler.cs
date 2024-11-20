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
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Cards.Commands.Activate
{
    public class ActivateCardCommandHandler : IRequestHandler<ActivateCardCommand, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateCardCommandHandler(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ActivateCardCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
                return Result.Fail("User with specified id not found");
            
            var codeHash = await Hasher.ComputeSha256HashAsync(request.CardCode);
            var isActivated =  await _unitOfWork.CardRepository.ActivateAsync(user.Id, request.CardNumber, codeHash);  

            if (!isActivated)
                return Result.Fail($"Failed to activate card with number: {request.CardNumber}. Card not found");

            var isChangesSaved = await _unitOfWork.SaveChangesAsync();

            return Result.OkIf(
                isChangesSaved,
                $"Card with number: {request.CardNumber} was activated, but failed to save changes to the database");
        }
    }
}