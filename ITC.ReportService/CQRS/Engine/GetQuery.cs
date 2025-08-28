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
                .OrderBy(c => c.Date)
                .ToList();
            var innerRing = result.Analyses.Where(a => a.InnerRingDefect != 0)
                .Select(x => new DefectHistoryDto { Probability = x.InnerRingDefect, Date = x.DateTime })
                .OrderBy(c => c.Date)
                .ToList();
            var misalignment = result.Analyses.Where(a => a.Misalignment != 0)
                .Select(x => new DefectHistoryDto { Probability = x.Misalignment, Date = x.DateTime })
                .OrderBy(c => c.Date)
                .ToList();
            var outerRingDefect = result.Analyses.Where(a => a.OuterRingDefect != 0)
                .Select(x => new DefectHistoryDto { Probability = x.OuterRingDefect, Date = x.DateTime })
                .OrderBy(c => c.Date)
                .ToList();
            var rollingElementsDefect = result.Analyses.Where(a => a.RollingElementsDefect != 0)
                .Select(x => new DefectHistoryDto { Probability = x.RollingElementsDefect, Date = x.DateTime })
                .OrderBy(c => c.Date)
                .ToList();
            var unbalance = result.Analyses.Where(a => a.Unbalance != 0)
                .Select(x => new DefectHistoryDto { Probability = x.Unbalance, Date = x.DateTime })
                .OrderBy(c => c.Date)
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
            dto.LastAnalyseDate = dto.Defects.Max(c => c.History.LastOrDefault()?.Date) ?? DateTime.Now;

            var cageTimeToDie = GetTimeToProbabilityOne(cage);
            var innerRingTimeToDie = GetTimeToProbabilityOne(innerRing);
            var misalignmentTimeToDie = GetTimeToProbabilityOne(misalignment);
            var outerRingDefectTimeToDie = GetTimeToProbabilityOne(outerRingDefect);
            var rollingElementsDefectTimeToDie = GetTimeToProbabilityOne(rollingElementsDefect);
            var unbalanceTimeToDie = GetTimeToProbabilityOne(unbalance);

            dto.RecommendedMaintenanceDate = new List<DateTime?>
                (
                    [
                        cageTimeToDie,
                        innerRingTimeToDie,
                        misalignmentTimeToDie,
                        outerRingDefectTimeToDie,
                        rollingElementsDefectTimeToDie,
                        unbalanceTimeToDie
                    ]
                ).Where(c => c != null)
                .Min();

            return dto;
        }).ToList();

        return new PageableCollection<EngineDto>(res, res.Count);
    }

    protected DateTime? GetTimeToProbabilityOne(List<DefectHistoryDto> points, int lastPointsCount = 100)
    {
        if (points == null || points.Count < 3)
            return null;
    
        var recentPoints = points.TakeLast(Math.Min(lastPointsCount, points.Count)).ToList();
    
        // Линеаризуем: преобразуем в ln(1/(1-p)) для экспоненциальной аппроксимации
        double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
        int n = recentPoints.Count;
    
        double baseDate = recentPoints[0].Date.ToOADate();
    
        foreach (var point in recentPoints)
        {
            double x = point.Date.ToOADate() - baseDate;
            // Преобразование для экспоненциального роста к 1
            double y = Math.Log(1.0 / (1.0 - point.Probability));
        
            sumX += x;
            sumY += y;
            sumXY += x * y;
            sumX2 += x * x;
        }
    
        // Линейная регрессия для преобразованных данных
        double b = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        double a = (sumY - b * sumX) / n;
    
        if (b <= 0)
            return null;
    
        // Обратное преобразование: находим когда p = 1
        // 1/(1-p) = exp(a + b*x) → 1-p = exp(-a - b*x) → p = 1 - exp(-a - b*x)
        // Для p = 1: exp(-a - b*x) = 0, что невозможно
        // Поэтому находим когда p достигнет 0.999 (практически 1)
        double targetY = Math.Log(1.0 / (1.0 - 0.999));
        double targetX = (targetY - a) / b;
    
        DateTime resultDate = recentPoints[0].Date.AddDays(targetX);
        return resultDate;
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