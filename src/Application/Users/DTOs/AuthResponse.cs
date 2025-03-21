using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Users.DTOs;

public record AuthResponse(
    string Token,
    string RefreshToken,
    string TokenType,
    int ExpiresIn);