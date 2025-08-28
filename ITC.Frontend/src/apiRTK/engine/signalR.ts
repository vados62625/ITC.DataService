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
}

const events: TSignalREvents = {
  ChangeState: ({ data, api }) => {
    api.dispatch(
      EngineApi.util.invalidateTags([
        { type: 'Engine', id: data.engineDto.id },
        { type: 'Engine', id: data.engineDto.id },
      ]),
    )
  },

  Delete: ({ data, api }) => {
    api.dispatch(
      EngineApi.util.invalidateTags([
        { type: 'Engine' },
        { type: 'Engine', id: data },
      ]),
    )
  },

  Add: ({ api }) => {
    api.dispatch(EngineApi.util.invalidateTags([{ type: 'Engine' }]))
  },
}

export const EngineHub = {
  hubUrl,
  events,
}
