using System.ComponentModel.DataAnnotations;

namespace ITC.CQRS.Base.Update;

public interface IUpdateEntityCommandBase
{
    /// <summary>
    /// ������������� ��������� ��������
    /// </summary>
    Guid Id { get; set; }
}

public abstract class UpdateEntityCommandBase : IUpdateEntityCommandBase
{
    [Required]
    public Guid Id { get; set; }
}