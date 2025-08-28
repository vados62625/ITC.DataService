import { SignalRTypes } from "../../store"
import { EngineDto } from "../../api"
import { EngineApi } from "./apiSlice"

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
    console.log('Invalidating after update:', data)
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
