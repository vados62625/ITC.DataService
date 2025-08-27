import type { SnackBarItemDefault } from '@consta/uikit/SnackBar'
import { createAsyncThunk } from '@reduxjs/toolkit'
import { v4 as uuidv4 } from 'uuid'

import { NotificationSlice } from '../model'

type AddNotificationArgs = {
  message: string
  status: 'alert' | 'success'
  id?: string
}

export const addNotification = createAsyncThunk<void, AddNotificationArgs>(
  'notification/add',
  ({ id, message, status }, api) => {
    const dispatch = api.dispatch
    const notificationId = id || uuidv4()
    const notification: SnackBarItemDefault = {
      key: notificationId,
      showProgress: 'line',
      autoClose: 2,
      message,
      status,
      onClose: () => dispatch(NotificationSlice.actions.deleteNotification(notificationId)),
    }
    dispatch(NotificationSlice.actions.addNotification(notification))
  },
)
