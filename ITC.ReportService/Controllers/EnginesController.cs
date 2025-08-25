using ITC.Domain.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITC.ReportService.Controllers;

[ApiController]
[Route("[controller]")]
public class EnginesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnginesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить список двигателей
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<IEnumerable<EngineDto>> Get(Domain.CQRS.Engine.GetQuery query)
    {
        var result = _mediator.Send(query);
        return Ok(result);
    }
    
    /// <summary>
    /// Добавить двигатель
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<EngineDto> Add(Domain.CQRS.Engine.AddCommand command)
    {
        var result = _mediator.Send(command);
        return CreatedAtAction(nameof(Add), result);
    }
    
    /// <summary>
    /// Изменить двигатель
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<EngineDto> Update(Domain.CQRS.Engine.UpdateCommand command)
    {
        var result = _mediator.Send(command);
        return Ok(result);
    }
    
    /// <summary>
    /// Удалить двигатель
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<EngineDto> Delete(Domain.CQRS.Engine.DeleteCommand command)
    {
        var result = _mediator.Send(command);
        return NoContent();
    }
}