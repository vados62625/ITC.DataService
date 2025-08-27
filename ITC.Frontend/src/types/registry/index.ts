import { EngineStatus } from "../engine"

export type RegistryRow = {
  id: string,
  name: string
  status: EngineStatus
  isLastAnalyseHasDefect: boolean
  lastAnalazeDate: string
}

export const TableFilterName = {
  LIVE :'В реальном времени',
  FILE : "Загруженные",
} as const

export type TableFilterType = keyof typeof TableFilterName
export type TableFilterNameType = typeof TableFilterName[keyof typeof TableFilterName];
