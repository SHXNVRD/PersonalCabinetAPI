using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Queries.GetById
{
    public record GetUserByIdQuery(string Id) : IRequest<Result<UserResponse>>; 
}