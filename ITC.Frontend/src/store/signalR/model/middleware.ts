import { HubConnectionState } from '@microsoft/signalr'
import { createListenerMiddleware } from '@reduxjs/toolkit'

import {
  isSignalRAction,
  isSignalRSendMessageAction,
  isSignalRStartConnectionAction,
  isSignalRStopConnectionAction,
  onRestartConnection,
  onSendMessage,
  onStartConnection,
  onStopConnection,
} from '../libs'

import type { TApi } from './types'
import { AppDispatch, RootState } from '../../../store/store'

export const listenerMiddleware = createListenerMiddleware()

/**
 * Отслеживание событий подписок signalR
 */
listenerMiddleware.startListening({
  predicate: isSignalRAction,
  effect: (action, api) => {
    const state = api.getState() as RootState
    const dispatch = api.dispatch as AppDispatch
    const hub = state.signalR[action.payload.hubUrl]

    // Создание подписки, если её ещё нет или рестарт
    if (isSignalRStartConnectionAction(action)) {
      if (!hub) {
        onStartConnection({
          hubUrl: action.payload.hubUrl,
          hubMethods: action.payload.hubMethods,
          dispatch,
          api: api as TApi,
        })
        return
      }

      if (hub.status === HubConnectionState.Disconnected) {
        onRestartConnection({ hub, dispatch })
      }
    }

    // Отправка данных по подписке
    if (isSignalRSendMessageAction(action)) {
      onSendMessage({
        hub,
        methodName: action.payload.methodName,
        data: action.payload.data,
      })
      return
    }

    // Остановка подписки
    if (isSignalRStopConnectionAction(action)) {
      onStopConnection({
        hub,
        dispatch,
      })
      return
    }
  },
})
