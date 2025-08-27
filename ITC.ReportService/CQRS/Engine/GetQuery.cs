using AutoMapper;
using ITC.Domain.CQRS.Base;
using ITC.Domain.Dto;
using ITC.Domain.Enums;
using ITC.Domain.Models;
using ITC.Storage.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.ReportService.CQRS.Engine;

public class GetQuery : PagingQueryBase, IRequest<PageableCollection<EngineDto>>
{
    public Guid? Id { get; set; }
    public EngineType? EngineType { get; set; }
    public string? Name { get; set; }
    public EngineStatus? EngineStatus { get; set; }
}

public class GetQueryHandler : IRequestHandler<GetQuery, PageableCollection<EngineDto>>
{
    private readonly IMapper _mapper;
    private readonly DbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetQueryHandler(IMapper mapper, DbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PageableCollection<EngineDto>> Handle(GetQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<Domain.Models.Engine>().Include(c => c.Analyses).AsQueryable();

        query = query.FilterBy(request);

        var results = await query.ToListAsync(cancellationToken);

        var res = results.Select(result =>
        {
            var dto = _mapper.Map<EngineDto>(result);
            var cage = result.Analyses.Where(a => a.CageDefect != 0)
                .Select(x => new DefectHistoryDto { Probability = x.CageDefect, Date = x.DateTime })
                .OrderBy(c  => c.Date)
                .ToList();
            var innerRing = result.Analyses.Where(a => a.InnerRingDefect != 0)
                .Select(x => new DefectHistoryDto { Probability = x.InnerRingDefect, Date = x.DateTime })
                .OrderBy(c  => c.Date)
                .ToList();
            var misalignment = result.Analyses.Where(a => a.Misalignment != 0)
                .Select(x => new DefectHistoryDto { Probability = x.Misalignment, Date = x.DateTime })
                .OrderBy(c  => c.Date)
                .ToList();
            var outerRingDefect = result.Analyses.Where(a => a.OuterRingDefect != 0)
                .Select(x => new DefectHistoryDto { Probability = x.OuterRingDefect, Date = x.DateTime})
                .OrderBy(c  => c.Date)
                .ToList();
            var rollingElementsDefect = result.Analyses.Where(a => a.RollingElementsDefect != 0)
                .Select(x => new DefectHistoryDto { Probability = x.RollingElementsDefect, Date = x.DateTime })
                .OrderBy(c  => c.Date)
                .ToList();
            var unbalance = result.Analyses.Where(a => a.Unbalance != 0)
                .Select(x => new DefectHistoryDto { Probability = x.Unbalance, Date = x.DateTime })
                .OrderBy(c  => c.Date)
                .ToList();
            dto.Defects = new List<DefectDto>()
            {
                new()
                {
                    EngineId = result.Id,
                    Type = DefectType.CAGE,
                    History = cage
                },
                new()
                {
                    EngineId = result.Id,
                    Type = DefectType.INNER_RING,
                    History = innerRing
                },
                new()
                {
                    EngineId = result.Id,
                    Type = DefectType.MISALIGNMENT,
                    History = misalignment
                },
                new()
                {
                    EngineId = result.Id,
                    Type = DefectType.OUTER_RING,
                    History = outerRingDefect
                },
                new()
                {
                    EngineId = result.Id,
                    Type = DefectType.ROLLING_ELEMENTS,
                    History = rollingElementsDefect
                },
                new()
                {
                    EngineId = result.Id,
                    Type = DefectType.UNBALANCE,
                    History = unbalance
                },
            };
            dto.IsLastAnalyseHasDefect = dto.Defects.Any(c => c.History.LastOrDefault()?.Probability > 0);
            dto.LastAnalyseDate = dto.Defects.Max(c => c.History.LastOrDefault()?.Date);
            return dto;
        }).ToList();

        return new PageableCollection<EngineDto>(res, res.Count);
    }
}

//
// public class GetQueryHandler : GetCollectionHandlerBase<GetQuery, Domain.Models.Engine, EngineDto>
// {
//     public GetQueryHandler(IMapper mapper, DbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(mapper, dbContext, httpContextAccessor)
//     {
//     }
//
//     public override Task<IQueryable<Domain.Models.Engine>> PreRequestAction(GetQuery request, IQueryable<Domain.Models.Engine> query, CancellationToken cancellationToken)
//     {
//         return Task.FromResult(query);
//     }
//
//     // public override async Task<PageableCollection<EngineDto>> PostRequestAction(GetQuery request, PageableCollection<EngineDto> collection, CancellationToken cancellationToken)
//     // {
//     //     // var analyses = await DbContext.Set<Analysis>()
//     //     //     .Where(c => c.EngineId == )
//     //     //     .ToListAsync();
//     // }
// }