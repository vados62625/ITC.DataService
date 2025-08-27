import type { InitialState } from './types'

export const initialState: InitialState = {
  mode: 'FILE',
  modal: {
    file: null,
    name: '',
    isOpen: false
  },
  pagination: {
    limit: 0,
    total: 0,
    curentPageNumber: 0
  }
}
