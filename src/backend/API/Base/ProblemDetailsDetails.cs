namespace API.Base;

public class ProblemDetailsDetails
{
    public const string BadRequestDetail = "The request is invalid";
    public const string UnauthorizedDetail = "Failed to log in. Authorization is required";
    public const string ForbiddenDetail = "Access to this resource is denied";
    public const string NotFoundDetail = "The requested resource was not found";
    public const string ConflictDetail = "Failed to create new resource";
    public const string InternalServerErrorDetail = "An internal server error has occurred";
}