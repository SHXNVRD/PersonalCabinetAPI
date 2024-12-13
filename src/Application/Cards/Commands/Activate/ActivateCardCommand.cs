using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Application.Cards.Commands.Activate
{
    public class ActivateCardCommand : IRequest<Result>
    {
        public string UserId { get; set; }
        public int CardNumber { get; set; }
        public string CardCode { get; set; }
    }
}