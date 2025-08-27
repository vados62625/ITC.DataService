import type { SnackBarItemDefault } from '@consta/uikit/SnackBar'
import { createListenerMiddleware, isAsyncThunkAction, isRejected } from '@reduxjs/toolkit'

import { NotificationSlice } from './slice'
import type { TApiAction } from './types'

export const listenerMiddleware = createListenerMiddleware()

/**
 * Отслеживание событий apiSlice на успешные/ошибочные запросы к серверу и добавление уведомлений об этом
 */
listenerMiddleware.startListening({
  predicate: action => {
    return isAsyncThunkAction(action)
  },
  effect: (action, api) => {
    const apiAction = action as typeof action & TApiAction
    const {
      message,
      actionType,
    } = apiAction.meta.baseQueryMeta ?? {}
    if (!message) {
      return
    }

    const newNotification: SnackBarItemDefault = {
      key: apiAction.meta.requestId,
      showProgress: 'line',
      autoClose: 2,
      message,
      status: isRejected(apiAction) ? 'alert' : 'success',
      onClose: () =>
        api.dispatch(NotificationSlice.actions.deleteNotification(apiAction.meta.requestId)),
    }
    api.dispatch(NotificationSlice.actions.addNotification(newNotification))
  },
})
