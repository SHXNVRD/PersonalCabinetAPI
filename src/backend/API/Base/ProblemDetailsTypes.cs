namespace API.Base;

public static class ProblemDetailsTypes
{
    public const string BadRequestType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1";
    public const string UnauthorizedType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2";
    public const string ForbiddenType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4";
    public const string NotFoundType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5";
    public const string ConflictType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10";
    public const string InternalServerErrorType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";
}