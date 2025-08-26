import React, { useRef } from 'react'
import { Loader } from '@consta/uikit/Loader'
import { useComponentSize } from '@consta/uikit/useComponentSize'
import ConfigProvider from 'antd/es/config-provider'
import ruLocale from 'antd/es/locale/ru_RU'
import AntdTable from 'antd/es/table'
import cn from 'classnames'

import { getRowEvents } from '../lib'
import type { TableProps, TRecord, TTableConfig } from '../model'

import styles from './styles.css'

// Стандартная высота шапки таблицы
const HEADER_HEIGHT = 41
// Уменьшение размера скрола таблицы, чтобы он не появлялся всегда
const WIDTH_OFFSET = 5

/* Переопределение текстов для таблицы */
if (ruLocale.Table) {
  ruLocale.Table.filterEmptyText = 'Ничего не найдено'
}

//https://ant.design/components/table#design-token
const TABLE_CONFIG: TTableConfig = {
  headerBg: 'var(--color-bg-system)',
  headerSplitColor: '',
  cellFontSize: 14,
  borderColor: 'var(--color-bg-border)',
  fixedHeaderSortActiveBg: 'var(--color-bg-system)',
  headerSortActiveBg: 'var(--color-bg-system)',
  cellPaddingBlock: 10,
  cellPaddingInline: 12,
  footerBg: 'var(--bg-color)',
}

const emptyRowEvents = {}

export const Table = <T extends TRecord>({
  virtual = true,
  pagination = false,
  rowKey = 'id',
  loading = false,
  dataTestId = 'table',
  scroll,
  expandable,
  themeTokens,
  headerHeight = HEADER_HEIGHT,
  expandNewItem,
  onRowClick,
  emptyText,
  ...props
}: TableProps<T>) => {
  const tableContainer = useRef<HTMLDivElement>(null)
  const { height, width } = useComponentSize(tableContainer)

  const childrenDataKey = expandable?.childrenColumnName ?? 'children'
  const tableHeight = props.tableHeight ? props.tableHeight : height - headerHeight
  const isSomeRowHasChildren =
    props.dataSource?.some(el => childrenDataKey in el) || Boolean(expandable)
  const tableScroll = virtual
    ? {
        y: scroll?.y ?? tableHeight,
        x: scroll?.x ?? width - WIDTH_OFFSET,
      }
    : {
        y: scroll?.y ?? tableHeight,
      }

  return (
    <div ref={tableContainer} data-testid={dataTestId} className={styles.container}>
      <ConfigProvider
        locale={ruLocale}
        theme={{
          token: themeTokens,
          components: { Table: { ...TABLE_CONFIG, ...props.tableConfig } },
        }}>
        <AntdTable
          {...props}
          rowKey={rowKey}
          style={{ minWidth: width, minHeight: tableHeight }}
          rootClassName={cn(styles.table, props.rootClassName, {
            [styles.virtualTable]: virtual,
            [styles.nonVirtualTable]: !virtual,
          })}
          pagination={pagination}
          loading={{
            indicator: (
              <div>
                <Loader data-testid="tableLoader" />
              </div>
            ),
            spinning: loading,
          }}
          locale={{ emptyText }}
          virtual={virtual}
          scroll={tableScroll}
          onRow={record => {
            return onRowClick ? getRowEvents(record, { onClick: onRowClick }) : emptyRowEvents
          }}
        />
      </ConfigProvider>
    </div>
  )
}
