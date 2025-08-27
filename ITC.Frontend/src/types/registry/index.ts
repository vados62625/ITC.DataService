import { EngineStatus } from "../engine"

export type RegistryRow = {
  id: string,
  name: string
  status: EngineStatus
  isLastAnalyseHasDefect: boolean
  lastAnalazeDate: string
}

export const TableFilterName = {
  LIVE :'Подключенные',
  FILE : "Загруженные",
} as const

export type TableFilterType = keyof typeof TableFilterName
export type TableFilterNameType = typeof TableFilterName[keyof typeof TableFilterName];
