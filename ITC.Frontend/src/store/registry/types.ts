import { TableFilterType } from "../../types"

export type InitialState = {
  modal: {
    file: File | null,
    name: string,
    isOpen: boolean
  },
  mode: TableFilterType
  pagination:{
    limit: number,
    total: number,
    curentPageNumber: number
  }
}