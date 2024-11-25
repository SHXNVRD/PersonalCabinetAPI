using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Application.Cards.Commands.Deactivate
{
    public class DeactivateCommandHandler : IRequestHandler<DeactivateCardCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateCommandHandler(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeactivateCardCommand request, CancellationToken cancellationToken)
        {
            var isDeactivated = await _unitOfWork.CardRepository.DeactivateAsync(request.CardNumber);
            
            if (!isDeactivated)
                return Result.Fail($"Failed to deactivate card with number: {request.CardNumber}");
            
            var isChangesSaved = await _unitOfWork.SaveChangesAsync();
            
            return Result.OkIf(
                isChangesSaved,
                $"Card with number: {request.CardNumber} was deactivated, but failed to save changes to the database");
        }
    }
}