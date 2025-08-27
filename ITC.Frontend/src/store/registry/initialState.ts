import type { InitialState } from './types'

export const initialState: InitialState = {
  mode: 'FILE',
  addModal: {
    file: null,
    name: '',
    isOpen: false
  },
  pagination: {
    limit: 0,
    total: 0,
    curentPageNumber: 0
  },
  removeModal: {
    isOpen: false,
    selectedEngine: null
  }
}
