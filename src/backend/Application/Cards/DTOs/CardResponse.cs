namespace Application.Cards.DTOs;

public record CardResponse(
    Guid Id,
    string Status,
    string Number,
    decimal Balance);