namespace Application.DTOs
{
    public class SaveChangesResult
    {
        public Exception? Exception { get; set; }
        public bool IsOk => Exception == null;
    }
}