namespace ITC.Domain.Dto;

public class PageableCollection<TItem> : IPageableCollection<TItem>
{
    public IReadOnlyCollection<TItem> Items { get; set; } = Array.Empty<TItem>();
    public int TotalCount { get; set; }

    public PageableCollection(){}
    public PageableCollection(IReadOnlyCollection<TItem> items, int totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }

    public static PageableCollection<TItem> Empty => new (Array.Empty<TItem>(), 0);
}

public interface IPageableCollection<TItem>
{
    IReadOnlyCollection<TItem> Items { get; set; }
    int TotalCount { get; set; }
}