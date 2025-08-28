import {
  HttpTransportType,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr'

import type { TApi, THubInfo, THubMethods } from '../model'
import { SignalRSlice } from '../model'
import { AppDispatch } from 'src/store/store'
import { formatDatesFromSignalR } from '../../../utils'

export const onStartConnection = ({
  hubUrl,
  hubMethods,
  dispatch,
  api,
}: {
  hubUrl: string
  dispatch: AppDispatch
  hubMethods: THubMethods
  api: TApi
}) => {
  const newHubConnection = new HubConnectionBuilder()
    .withUrl(hubUrl, {
      transport: HttpTransportType.WebSockets,
      accessTokenFactory: () => localStorage.getItem("token") ?? '',
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build()

  for (const methodName in hubMethods) {
    newHubConnection.on(methodName, (data: never) =>
      hubMethods[methodName]({ data: formatDatesFromSignalR(data) as never, api }),
    )
  }

  newHubConnection.onclose(() => {
    dispatch(
      SignalRSlice.actions.updateConnection({
        status: HubConnectionState.Disconnected,
        hubUrl,
      }),
    )
  })

  newHubConnection.onreconnecting(() => {
    dispatch(
      SignalRSlice.actions.updateConnection({
        status: HubConnectionState.Reconnecting,
        hubUrl,
      }),
    )
  })

  newHubConnection.onreconnected(() => {
    dispatch(
      SignalRSlice.actions.updateConnection({
        id: newHubConnection.connectionId ?? '',
        status: HubConnectionState.Connected,
        hubUrl,
      }),
    )
  })

  newHubConnection
    .start()
    .then(() => {
      dispatch(
        SignalRSlice.actions.addConnection({
          id: newHubConnection.connectionId ?? '',
          hubUrl,
          status: HubConnectionState.Connected,
          connection: newHubConnection,
        }),
      )
    })
    .catch((e: unknown) => {
      console.error('Ошибка подключения к SignalR', e)
    })
}

export const onSendMessage = async ({
  hub,
  methodName,
  data,
}: {
  hub?: THubInfo
  methodName: string
  data: unknown
}) => {
  if (!hub || !methodName) {
    return
  }

  await hub.connection.invoke(methodName, data).catch((e: unknown) => {
    console.error('Ошибка SignalR', e)
  })
}

export const onStopConnection = ({ hub, dispatch }: { hub?: THubInfo; dispatch: AppDispatch }) => {
  if (!hub) {
    return
  }

  hub.connection
    .stop()
    .then(() => {
      dispatch(
        SignalRSlice.actions.updateConnection({
          status: HubConnectionState.Disconnected,
          hubUrl: hub.hubUrl,
        }),
      )
    })
    .catch((e: unknown) => {
      console.error('Ошибка SignalR', e)
    })
}

export const onRestartConnection = ({
  hub,
  dispatch,
}: {
  hub?: THubInfo
  dispatch: AppDispatch
}) => {
  if (!hub) {
    return
  }

  hub.connection
    .start()
    .then(() => {
      dispatch(
        SignalRSlice.actions.updateConnection({
          status: HubConnectionState.Connected,
          hubUrl: hub.hubUrl,
        }),
      )
    })
    .catch((e: unknown) => {
      console.error('Ошибка SignalR', e)
    })
}
