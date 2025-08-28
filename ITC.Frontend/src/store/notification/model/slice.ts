import type { SnackBarItemDefault } from '@consta/uikit/SnackBar'
import type { PayloadAction } from '@reduxjs/toolkit'
import { createSlice } from '@reduxjs/toolkit'

type TInitialState = {
  items: Record<string, SnackBarItemDefault>
}

const initialState: TInitialState = {
  items: {},
}

export const NotificationSlice = createSlice({
  name: 'notification',
  initialState,
  reducers: {
    setInitialState: () => initialState,
    addNotification: (state, action: PayloadAction<SnackBarItemDefault>) => {
        state.items[action.payload.key] = action.payload
    },
    deleteNotification: (state, action: PayloadAction<SnackBarItemDefault['key']>) => {
      /* C Map работать не будет, дока RTK советуют не использовать Несериализуемые значения
       * https://redux.js.org/style-guide/#do-not-put-non-serializable-values-in-state-or-actions
       * */

      const isNotificationExist = Boolean(state.items[action.payload])
      if (isNotificationExist) {
        // eslint-disable-next-line @typescript-eslint/no-dynamic-delete
        delete state.items[action.payload]
      }
    },
  },
  selectors: {
    notificationsSelector: state => state.items,
  },
})
