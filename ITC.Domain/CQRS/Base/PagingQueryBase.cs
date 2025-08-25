namespace ITC.Domain.CQRS.Base;

public class PagingQueryBase : IPagingQuery
{
    public int Page { get; set; } = 1;
    public int ItemsCount { get; set; } = int.MaxValue;
}

public interface IPagingQuery
{
    public int Page { get; set; }
    public int ItemsCount { get; set; }
}