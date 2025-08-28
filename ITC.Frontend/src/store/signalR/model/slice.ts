import type { PayloadAction } from '@reduxjs/toolkit'
import { createSlice } from '@reduxjs/toolkit'

import type { THubInfo, TInitialState, TSignalRUpdateConnectionAction } from './types'

const initialState: TInitialState = {}

export const SignalRSlice = createSlice({
  name: 'signalR',
  initialState,
  reducers: {
    addConnection: (state, action: PayloadAction<THubInfo>) => {
      state[action.payload.hubUrl] = action.payload
    },
    updateConnection: (state, action: PayloadAction<TSignalRUpdateConnectionAction>) => {
      const connection = state[action.payload.hubUrl]

      if (connection) {
        connection.status = action.payload.status
        connection.id = action.payload.id || connection.id || ''
      }
    },
  },
  selectors: {
    hubSelector: (state) => state
  },
})
