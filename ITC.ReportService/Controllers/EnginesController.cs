using ITC.Domain.Dto;
using ITC.ReportService.CQRS.Engine;
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
    public async Task<ActionResult<IEnumerable<EngineDto>>> Get(GetQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Добавить двигатель
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<EngineDto>> Add(AddCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Add), result);
    }

    /// <summary>
    /// Изменить двигатель
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<EngineDto>> Update(UpdateCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Удалить двигатель
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<ActionResult<EngineDto>> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCommand { Id = id });
        return NoContent();
    }
}