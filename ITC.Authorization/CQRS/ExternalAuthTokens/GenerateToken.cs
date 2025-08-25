using ITC.Authorization.Storage;
using ITC.CQRS.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Internal;

namespace ITC.Authorization.CQRS.ExternalAuthTokens;

public class GenerateToken : IRequest<ExternalAuthToken>
{
    public required string JwtRefresh { get; set; }

    public class Handler : IRequestHandler<UserApiRequestWrapper<GenerateToken, ExternalAuthToken>, ExternalAuthToken>
    {
        private readonly ServiceDbContext _db;
        private readonly ISystemClock _clock;

        public Handler(ServiceDbContext db, ISystemClock clock)
        {
            _db = db;
            _clock = clock;
        }

        public async Task<ExternalAuthToken> Handle(UserApiRequestWrapper<GenerateToken, ExternalAuthToken> request, CancellationToken cancellationToken)
        {
            var token = await _db
                .Set<ExternalAuthToken>()
                .FirstOrDefaultAsync(c => c.UserId == request.UserContext.Id, cancellationToken);
            
            token ??= new ExternalAuthToken()
            {
                UserId = request.UserContext.Id,
            };

            token.ValidTo = _clock.UtcNow.AddMinutes(10);
            token.Token = Guid.NewGuid();
            token.JwtRefresh = request.Request.JwtRefresh;

            _db.Set<ExternalAuthToken>().Update(token);

            await _db.SaveChangesAsync(cancellationToken);
            return token;
        }
    }
}