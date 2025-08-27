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
    changeAddModal: (state, action: PayloadAction<Partial<InitialState['addModal']>>) => {
      state.addModal = {...state.addModal, ...action.payload}
    },
    changeRemoveModal: (state, action: PayloadAction<Partial<InitialState['removeModal']>>) => {
      state.removeModal = {...state.removeModal, ...action.payload}
    },
  },
  selectors: {
    mode: state => state.mode,
    addModal: state => state.addModal,
    removeModal: state => state.removeModal,
  },
})
