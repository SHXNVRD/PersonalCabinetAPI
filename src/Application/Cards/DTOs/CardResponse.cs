using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Cards.DTOs;

public record CardResponse(
    Guid Id,
    string Status,
    string Number,
    decimal Balance);