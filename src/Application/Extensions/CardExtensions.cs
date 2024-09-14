using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Models;

namespace Application.Extensions
{
    public static class CardExtensions
    {
        public static CardResponse ToDto(this Card card)
        {
            return new CardResponse()
            {
                Id = card.Id
            };
        }
    }
}