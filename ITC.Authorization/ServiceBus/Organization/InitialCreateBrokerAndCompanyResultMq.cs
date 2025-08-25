using ITC.Authorization.Storage;
using ITC.ServiceBus.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITC.Authorization.ServiceBus.Organization;

public class InitialCreateBrokerAndCompanyResultMq
{
    public required Guid RegistrationRequestId { get; set; }
    public Guid BrokerId { get; set; }
    public Guid CompanyId { get; set; }

    public class Handler : IServiceBusMessageHandler<InitialCreateBrokerAndCompanyResultMq>
    {
        private readonly ServiceDbContext _db;

        public Handler (ServiceDbContext db)
        {
            _db = db;
        }
        public async Task Handle(InitialCreateBrokerAndCompanyResultMq message, IDictionary<string, string> headers, DateTimeOffset timestamp,
            CancellationToken cancellationToken)
        {
            var entity = await _db.Set<InitialBrokerRegistrationRequest>()
                .FirstAsync(c => c.Id == message.RegistrationRequestId, cancellationToken);

            entity.CompanyId = message.CompanyId;
            entity.BrokerId = message.BrokerId;

            _db.Set<InitialBrokerRegistrationRequest>().Update(entity);

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}