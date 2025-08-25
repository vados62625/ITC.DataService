namespace ITC.Domain.Dto;

public abstract class EntityDtoBase : IEntityDto
{
    /// <summary>
    /// Id
    /// </summary>
    public virtual Guid Id { get; set; }

    /// <summary>
    /// Дата/Время удаления
    /// </summary>
    public virtual DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// Дата/Время последнего обновления
    /// </summary>
    public virtual DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Дата/Время создания
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; }
}