using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using ITC.Domain.Dto;
using ITC.Domain.Enums;
using ITC.ReportService.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.ReportService.CQRS.Engine;

public class AddCommand
{
    public string? Name { get; set; }
}

public class AddCommandWrapper : IRequest<EngineDto>
{
    public AddCommandWrapper(AddCommand command, IFormFile file)
    {
        Command = command;
        File = file;
    }

    public AddCommand Command { get; set; }
    public IFormFile File { get; set; }

    public class AddCommandHandler : IRequestHandler<AddCommandWrapper, EngineDto>
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataServiceClient _dataServiceClient;

        public AddCommandHandler(IMapper mapper, DbContext dbContext, IHttpContextAccessor httpContextAccessor,
            IDataServiceClient dataServiceClient)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _dataServiceClient = dataServiceClient;
        }

        public async Task<EngineDto> Handle(AddCommandWrapper request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                throw new ValidationException("No file uploaded");
            }
            
            var dbSet = _dbContext.Set<Domain.Models.Engine>();
            var query = dbSet.AsQueryable();

            var entity = _mapper.Map<Domain.Models.Engine>(request.Command);
            entity.EngineType = EngineType.FromFile;
            entity.EngineStatus = EngineStatus.Pending;
            await dbSet.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            var dto = await query
                .Where(c => c.Id == entity.Id)
                .ProjectTo<EngineDto>(_mapper.ConfigurationProvider)
                .FirstAsync(cancellationToken);

            await _dataServiceClient.UploadCsv(request.File);
            return dto;
        }
    }

    public class AddCommandValidator : AbstractValidator<AddCommand>
    {
        public AddCommandValidator(DbContext db)
        {
        }
    }
}