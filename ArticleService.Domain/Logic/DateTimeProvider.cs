using ArticleService.Domain.Ports;

namespace ArticleService.Domain.Logic
{
    public class DateTimeProvider : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
