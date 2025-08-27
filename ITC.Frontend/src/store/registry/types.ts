import { RegistryRow, TableFilterType } from "../../types"

export type InitialState = {
  addModal: {
    file: File | null,
    name: string,
    isOpen: boolean
  },
  removeModal: {
    isOpen: boolean
    selectedEngine: RegistryRow | null
  },
  mode: TableFilterType
  pagination:{
    limit: number,
    total: number,
    curentPageNumber: number
  }
}