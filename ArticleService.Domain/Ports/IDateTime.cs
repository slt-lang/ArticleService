namespace ArticleService.Domain.Ports
{
    public interface IDateTime
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateTime Today { get; }
        DateTime UtcToday { get; }
    }
}
