import type { Action } from '@reduxjs/toolkit'

import type {
  TSignalRAction,
  TSignalRSendMessageAction,
  TSignalRStartConnectionAction,
  TSignalRStopConnectionAction,
} from '../model'

export const signalRPrefixEvent = 'signalR'

export const signalREvents = {
  start: `${signalRPrefixEvent}/startConnection`,
  stop: `${signalRPrefixEvent}/stopConnection`,
  send: `${signalRPrefixEvent}/sendMessage`,
} as const


export const isSignalRAction = (action: Action): action is TSignalRAction =>
  action.type.startsWith(signalRPrefixEvent)

export const isSignalRStartConnectionAction = (
  action: Action,
): action is TSignalRStartConnectionAction => action.type === signalREvents.start

export const isSignalRStopConnectionAction = (
  action: Action,
): action is TSignalRStopConnectionAction => action.type === signalREvents.stop

export const isSignalRSendMessageAction = (action: Action): action is TSignalRSendMessageAction =>
  action.type === signalREvents.send
