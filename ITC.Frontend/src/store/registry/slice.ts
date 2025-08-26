import type { PayloadAction } from '@reduxjs/toolkit'
import { createSlice } from '@reduxjs/toolkit'
import type { InitialState } from './types'
import { initialState } from './initialState'

export const RegistrySlice = createSlice({
  name: 'registry',
  initialState,
  reducers: {
    changeMode: (state, action: PayloadAction<InitialState['mode']>) => {
      state.mode = action.payload
    },
  },
  selectors: {
    mode: state => state.mode,
  },
})
