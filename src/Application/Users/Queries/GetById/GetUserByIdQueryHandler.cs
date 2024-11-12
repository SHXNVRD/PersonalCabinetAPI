using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Extensions;
using Application.Interfaces;
using Application.Interfaces.Token;
using Application.Users.DTOs;
using Domain.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Queries.GetById
{
    public class GetUserByTokenQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
    {
        private readonly UserManager<User> _userManager;

        public GetUserByTokenQueryHandler(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
        }

        public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            
            if (user == null)
                return Result.Fail($"User with specified id not found");

            return Result.Ok(user.ToDto());
        }
    }
}