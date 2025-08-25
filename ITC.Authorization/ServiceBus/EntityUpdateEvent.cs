namespace ITC.Authorization.ServiceBus;

public class EntityUpdateEvent
{
    public string EntityType { get; set; } // Тип сущности: Person, Employee
    public string ActionType { get; set; } // Действие: Add, Update, Delete
    // todo: Можно в будущем заюзать
    //public Guid RequestId { get; set; } // Уникальный идентификатор запроса
    //public string SourceSystem { get; set; } // Система-отправитель: auth, nsi
    //public string DestinationSystem { get; set; } // Система-получатель: auth, nsi
    //public DateTime Timestamp { get; set; } // Время события
    public string Payload { get; set; } // Данные сущности
}