using EventTicketing.Application.Common.Interface.Services;

namespace EventTicketing.Infrastructure.Common.Helpers;
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
