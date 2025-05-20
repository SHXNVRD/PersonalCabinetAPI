using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Configurations.Converters;

public class ToUtcValueConverter : ValueConverter<DateTime?, DateTime?>
{
    public ToUtcValueConverter() : base(
        dateTime => dateTime.HasValue
            ? dateTime.Value.Kind == DateTimeKind.Utc
                ? dateTime
                : DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc)
            : null,
        dateTime => dateTime.HasValue
            ? dateTime.Value.Kind == DateTimeKind.Utc
                ? dateTime
                : DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc)
            : null)
    { }
}