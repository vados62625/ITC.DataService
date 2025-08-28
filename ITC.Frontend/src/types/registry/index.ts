import { EngineStatus } from "../engine"

export type RegistryRow = {
  id: string,
  name: string
  status: EngineStatus
  isLastAnalyseHasDefect: boolean
  isLastAnalyseHasCriticalDefect: boolean
  lastAnalazeDate: string
  recommendedMaintenanceDate: Date | undefined
}

export const TableFilterName = {
  LIVE :'Подключенные',
  FILE : "Загруженные",
} as const

export type TableFilterType = keyof typeof TableFilterName
export type TableFilterNameType = typeof TableFilterName[keyof typeof TableFilterName];
