namespace ITC.Storage.Query;

public interface IEntityQuery : IPagingQuery
{
    /// <summary>
    /// Идентификатор записи
    /// </summary>
    Guid? Id { get; set; }

    /// <summary>
    /// Период выборки
    /// </summary>
    DateTimeOffset? CreatedAtFrom { get; set; }

    /// <summary>
    /// Период выборки
    /// </summary>
    DateTimeOffset? CreatedAtTo { get; set; }

    long? Timestamp { get; set; }
}
