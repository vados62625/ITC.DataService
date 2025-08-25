using System.Security.Claims;
using System.Text.Json;
using ITC.Authorization.CQRS.InviteUsers;
using ITC.Authorization.CQRS.InviteUsers.Dto;
using ITC.Authorization.Options;
using ITC.Authorization.ServiceBus.Organization;
using ITC.Authorization.Services;
using ITC.CQRS.Base.Get;
using ITC.ServiceBus.Interfaces;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Roles;
using MediatR;
using Microsoft.Extensions.Options;
using NLog;
using ILogger = NLog.ILogger;

namespace ITC.Authorization.ServiceBus;

public class EntityUpdateEventResult
{
    public string EntityType { get; set; } // Тип сущности: Employee
    public string ActionType { get; set; } // Действие: Add, Update, Delete
    // todo: Можно в будущем заюзать
    //public Guid RequestId { get; set; } // Уникальный идентификатор запроса
    //public string SourceSystem { get; set; } // Система-отправитель: auth, nsi
    //public string DestinationSystem { get; set; } // Система-получатель: auth, nsi
    //public DateTime Timestamp { get; set; } // Время события
    public string Payload { get; set; } // Данные сущности

    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    public class Handler : IServiceBusMessageHandler<EntityUpdateEventResult>
    {
        private readonly IServiceBusMessagePublisher<EntityUpdateEvent> _entityUpdateEventMqPublisher;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly KeycloakClient _keycloak;
        private readonly KeycloakClientOptions _clientOptions;
        private readonly IUserRolesCacheService _userRolesCacheService;

        public Handler(
            IServiceBusMessagePublisher<EntityUpdateEvent> entityUpdateEventMqPublisher,
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor,
            KeycloakClient keycloak,
            IOptions<KeycloakClientOptions> options,
            IUserRolesCacheService userRolesCacheService)
        {
            _entityUpdateEventMqPublisher = entityUpdateEventMqPublisher;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _keycloak = keycloak;
            _clientOptions = options.Value;
            _userRolesCacheService = userRolesCacheService;
        }
        public async Task Handle(EntityUpdateEventResult message, IDictionary<string, string> headers, DateTimeOffset timestamp,
            CancellationToken cancellationToken)
        {
            switch (message.EntityType)
            {
                case "Employee":
                    await HandleEmployeeEvent(message, cancellationToken);
                    break;
                default:
                    Log.Error($"Unhandled entity type: {message.EntityType}");
                    break;
            }
        }

        private async Task HandleEmployeeEvent(EntityUpdateEventResult message, CancellationToken cancellationToken)
        {
            switch (message.ActionType)
            {
                case "AddEmployeeFromInvitedUserMqResult":
                    await AddEmployeeFromInvitedUserMqResultHandler(message.Payload, cancellationToken);
                    break;
                default:
                    Log.Error($"Unhandled action type: {message.ActionType}");
                    break;
            }
        }

        private async Task AddEmployeeFromInvitedUserMqResultHandler(string payload, CancellationToken cancellationToken)
        {
            var message = JsonSerializer.Deserialize<AddEmployeeFromInvitedUserMqResult>(payload);
            if (message == null)
            {
                Log.Error($"Unsupported payload for {nameof(AddEmployeeFromInvitedUserMqResult)}");
                return;
            }

            var invitedUserQuery = new Get
            {
                InviteToken = message.InviteToken
            };

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, message.EmployeeId.ToString())
            }));

            _httpContextAccessor.HttpContext = defaultHttpContext;

            var request2 = new UserApiGetCollectionRequestWrapper<Get, InvitedUserDto>(invitedUserQuery, _httpContextAccessor.HttpContext);
            var invitedUsers = await _mediator.Send(request2, cancellationToken);
            if (invitedUsers == null)
            {
                Log.Error($"Не найден приглашенный пользователь с EmployeeId:{message.EmployeeId}");
                return;
            }

            var invitedUser = invitedUsers.Items.FirstOrDefault();
            if (invitedUser == null)
            {
                Log.Error($"Не найден приглашенный пользователь с EmployeeId: {message.EmployeeId}");
                return;
            }

            // var command = new AddEmployeeFromInvitedUserMq
            // {
            //     
            //     UserId = message.UserId,
            //     CompanyId = message.CompanyId,
            //     BrokerId = message.BrokerId,
            //     Phone = message.Phone,
            //     Position = invitedUser.Position,
            //     Email = invitedUser.Email,
            // };
            //
            // await _entityUpdateEventMqPublisher.Produce(new EntityUpdateEvent
            // {
            //     ActionType = "Add",
            //     EntityType = "Employee",
            //     Payload = JsonSerializer.Serialize(command)
            // });

            await AddUserRole(invitedUser.Id, invitedUser.Email, cancellationToken);
        }

        public async Task AddUserRole(Guid userId, string email, CancellationToken cancellationToken)
        {
            var users = await _keycloak.GetUsersAsync(_clientOptions.Realm, email: email, cancellationToken: cancellationToken);
            var rolesFromKeycloak = await _keycloak.GetRolesAsync(_clientOptions.Realm, _clientOptions.ClientId.ToString(), cancellationToken: cancellationToken);
            
            var employeeRole = new List<Role> {
                new Role
                {
                    Id = rolesFromKeycloak.First(c => c.Name == "user").Id,
                    Name = "user"
                }
            }.ToArray();

            var isSuccess = await _keycloak.AddClientRoleMappingsToUserAsync(_clientOptions.Realm,
                users.First().Id.ToString(),
                _clientOptions.ClientId.ToString(),
                employeeRole,
                cancellationToken);
            if (isSuccess)
            {
                await _userRolesCacheService.ProcessUserRoles(users.First().Id, cancellationToken);
            }
        }
    }
}
