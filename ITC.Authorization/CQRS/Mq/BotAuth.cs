using ITC.Authorization.CQRS.Mq.Base;
using ITC.Authorization.Storage;
using Microsoft.EntityFrameworkCore;

namespace ITC.Authorization.CQRS.Mq;

public class Authorized
{
    public required string RefreshToken { get; set; }
}

public class BotAuth
{
    public Guid BotAuthToken { get; set; }
    
    public class Handler: MessageHandlerBase<BotAuth>
    {
        private readonly ServiceDbContext _db;
        // private readonly IYandexMq _mq;

        public Handler(ServiceDbContext db)
        {
            _db = db;
            // _mq = mq;
        }

        protected override async Task Handle(BotAuth message, CancellationToken cancellationToken)
        {
            var token = await _db
                .Set<ExternalAuthToken>()
                .FirstOrDefaultAsync(c => c.Token == message.BotAuthToken, cancellationToken);

            if (token == null)
            {
                Log.Info("Token {0} not found", message.BotAuthToken);
                return;
            }


            var authorized = new Authorized()
            {
                RefreshToken = token.JwtRefresh
            };

            // await _mq.SendMessage(authorized, cancellationToken);
            
            token.Token = Guid.NewGuid();

            _db.Set<ExternalAuthToken>().Update(token);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}