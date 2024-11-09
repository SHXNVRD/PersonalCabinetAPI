namespace Infrastructure.Services
{
    public class EmailOptions
    {
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SmtpServer { get; set; } = string.Empty;
        public bool UseSsl { get; set; }
        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}