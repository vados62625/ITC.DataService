namespace ITC.Domain.Dto;

public interface IEntityDto
{
    /// <summary>
    /// Id
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Дата/Время удаления
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// Дата/Время последнего обновления
    /// </summary>
    DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Дата/Время создания
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }
}