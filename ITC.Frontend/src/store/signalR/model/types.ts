import type { HubConnection, HubConnectionState } from '@microsoft/signalr'
import type {
  ListenerEffectAPI,
  PayloadAction,
  ThunkDispatch,
  UnknownAction,
} from '@reduxjs/toolkit'

import { RootState } from '../../../store/store'
import { signalREvents } from '../libs/guards'

export type TInitialState = Record<string, THubInfo | undefined>

export type THubInfo = {
  id: string
  hubUrl: string
  status: HubConnectionState
  connection: HubConnection
}

export type TSignalRAction =
  | TSignalRSendMessageAction
  | TSignalRStartConnectionAction
  | TSignalRStopConnectionAction

export type TSignalRStartConnectionAction = PayloadAction<
  Pick<TSignalRPayload, 'hubUrl'> & { hubMethods: THubMethods },
  typeof signalREvents.start
>

export type TSignalRStopConnectionAction = PayloadAction<
  Pick<TSignalRPayload, 'hubUrl'>,
  typeof signalREvents.stop
>

export type TSignalRSendMessageAction = PayloadAction<TSignalRPayload, typeof signalREvents.send>

export type TSignalRPayload = {
  hubUrl: string
  data: unknown
  methodName: string
}

export type TSignalRUpdateConnectionAction = Pick<THubInfo, 'hubUrl' | 'status'> & {
  id?: string
}

export type TApi = ListenerEffectAPI<unknown, ThunkDispatch<RootState, unknown, UnknownAction>>

type TEventArgs<T> = {
  // данные которые пришли в подписке
  data: T
  api: TApi
}

export type THubEvent<T = never> = (args: TEventArgs<T>) => void | Promise<void>
export type THubMethods = Record<string, THubEvent>
