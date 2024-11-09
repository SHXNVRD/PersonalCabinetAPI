namespace Application.DTOs.Emails
{
    public record EmailAddress(string Address, string DisplayName)
    {
        public static implicit operator EmailAddress(string address) => new(address, string.Empty);
    }
}