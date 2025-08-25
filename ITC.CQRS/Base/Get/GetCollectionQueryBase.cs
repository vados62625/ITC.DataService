using ITC.Storage.Attributes;
using ITC.Storage.Query;

namespace ITC.CQRS.Base.Get;

/// <summary>
/// Базовый класс запроса коллекции объектов из БДБазовый класс запроса коллекции объектов из БД
/// </summary>
public abstract class GetCollectionQueryBase : IEntityQuery

{
    
    public int? Page { get; set; }
    public int? ItemsOnPage { get; set; }
    [Sort("CreatedAt")]
    public SortDirection? CreatedAtSortDirection { get; set; }
    [Sort("UpdatedAt")]
    public SortDirection? UpdatetAtSortDirection { get; set; }
    public Guid? Id { get; set; }
    public DateTimeOffset? CreatedAtFrom { get; set; }
    public DateTimeOffset? CreatedAtTo { get; set; }
    public long? Timestamp { get; set; }
}