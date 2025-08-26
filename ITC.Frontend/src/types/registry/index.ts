export type RegistryRow = {
  id: string,
  name: string
  status: StatusType
  isHasDefect: boolean
  lastAnalazeDate: string
}

type StatusType = 'new' |'pending' | 'failed' | 'succses'

export type TableType = 'Live' | 'File'