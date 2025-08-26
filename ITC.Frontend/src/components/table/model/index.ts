import type { ThemeConfig } from 'antd/es/config-provider/context'
import type { TableProps as AntdTableProps } from 'antd/es/table'
import type { ComponentToken as TableComponentToken } from 'antd/es/table/style'

export type TRecord = object
export type TTableConfig = Partial<TableComponentToken>

export type TableProps<D extends TRecord> = Omit<
  AntdTableProps<D>,
  'locale' | 'loading' | 'onRow' | 'data' | 'dataSource'
> & {
  tableConfig?: TTableConfig
  tableHeight?: string | number
  themeTokens?: ThemeConfig['token']
  headerHeight?: number
  expandNewItem?: boolean
  loading?: boolean
  onRowClick?: (record: D) => void
  dataSource?: D[]
  dataTestId?: string
  emptyText?: React.ReactNode
}
