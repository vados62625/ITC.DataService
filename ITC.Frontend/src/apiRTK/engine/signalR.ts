import { SignalRTypes } from "../../store"
import { EngineDto, IPageableCollectionOfEngineDto, PageableCollectionOfEngineDto } from "../../api"
import { EngineApi } from "./apiSlice"
import { addNotification, NotificationSlice } from "../../store/notification"
import { RootState } from "../../store/store"
import { AddNotificationArgs } from "../../store/notification/action"

const hubUrl = 'http://89.108.73.166:5016/engineHub'

type TChangeStateEventData = {
  engineDto: EngineDto
}

// описание событий в signalR hub
export type TSignalREvents = {
  ChangeState: SignalRTypes.THubEvent<TChangeStateEventData>
  Add: SignalRTypes.THubEvent<EngineDto>
  Delete: SignalRTypes.THubEvent<string>
  engineupdate: SignalRTypes.THubEvent<string>
}

const events: TSignalREvents = {
  ChangeState: ({ data, api }) => {
    console.log('Invalidating engine:', data.engineDto.id)
    api.dispatch(
      EngineApi.util.invalidateTags([
        { type: 'Engine', id: data.engineDto.id },
        { type: 'Engine', id: 'LIST' }, // Добавляем инвалидацию списка
      ]),
    )
  },

  Delete: ({ data, api }) => {
    console.log('Invalidating deleted engine:', data)
    api.dispatch(
      EngineApi.util.invalidateTags([
        { type: 'Engine', id: data },
        { type: 'Engine', id: 'LIST' },
      ]),
    )
  },

  Add: ({ api, data }) => {
    console.log('Invalidating after add:', data)
    api.dispatch(
      EngineApi.util.invalidateTags([
        { type: 'Engine', id: 'LIST' }
      ])
    )
  },

  engineupdate: ({ api, data }) => {
    // // Тайпгард для проверки, что объект является EngineDto
    // function isEngineDto(data: any): data is EngineDto {
    //   return data &&
    //     typeof data === 'object' &&
    //     // Проверяем опциональные свойства
    //     (data.isLastAnalyseHasDefect === undefined || typeof data.isLastAnalyseHasDefect === 'boolean') &&
    //     (data.isLastAnalyseHasCriticalDefect === undefined || typeof data.isLastAnalyseHasCriticalDefect === 'boolean') &&
    //     (data.name === undefined || typeof data.name === 'string') &&
    //     (data.engineStatus === undefined || typeof data.engineStatus === 'number') &&
    //     (data.engineType === undefined || typeof data.engineType === 'number') &&
    //     (data.defects === undefined || Array.isArray(data.defects)) &&
    //     (data.lastAnalyseDate === undefined || data.lastAnalyseDate instanceof Date) &&
    //     (data.recommendedMaintenanceDate === undefined || data.recommendedMaintenanceDate instanceof Date) &&
    //     (data.id === undefined || typeof data.id === 'string') &&
    //     (data.deletedAt === undefined || data.deletedAt instanceof Date) &&
    //     (data.updatedAt === undefined || data.updatedAt instanceof Date) &&
    //     (data.createdAt === undefined || data.createdAt instanceof Date);
    // }

    // if (isEngineDto(data)) {
    //   const engine = data
    //   api.dispatch(
    //     addNotification({
    //       status: 'success',
    //       message: `Данные о двигателе ${engine.name} успешно добавлены`,
    //     })
    //   );
    // }

    const state = api.getState() as RootState

    // Типгард функция для проверки структуры данных
    function isPageableCollectionOfEngineDto(data: any): data is IPageableCollectionOfEngineDto {
      return data &&
        (data.items === undefined || Array.isArray(data.items)) &&
        (data.totalCount === undefined || typeof data.totalCount === 'number');
    }

    if (isPageableCollectionOfEngineDto(data)) {
      const typedData = data;
      if (typedData.items && typedData.items.length > 0) {
        const engine = typedData.items[0];
        const notificationId = engine.id ?? 'notificationId'
        const notification: AddNotificationArgs = {
          id: notificationId,
          status: 'alert',
          message: `В двигателе ${engine.name} обнаружен дефект критической степени развития`,
        }

        if (!state.notification.items[notificationId]) {
          api.dispatch(
            addNotification(notification)
          );
        }
      }
    }

    api.dispatch(
      EngineApi.util.invalidateTags([
        { type: 'Engine', id: 'LIST' }
      ])
    )
  },
}

export const EngineHub = {
  hubUrl,
  events,
}
