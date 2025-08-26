import { TableType } from "../../types"

export type InitialState = {
  modal: {
    file: File | null,
    name: string,
    isOpen: boolean
  },
  mode: TableType
  pagination:{
    limit: number,
    total: number,
    curentPageNumber: number
  }
}