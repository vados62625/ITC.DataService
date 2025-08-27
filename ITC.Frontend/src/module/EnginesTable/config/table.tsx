import React from 'react'
import type { ColumnsType } from 'antd/es/table'

import { CONTEXT_ACTION, CONTEXT_ITEMS } from './contextButton'
import { Badge, BadgePropStatus } from '@consta/uikit/Badge'

import styles from './styles.css'
import { RegistryRow, TableFilterType } from '../../../types'
import { ContextButton } from '../../../components'
import { getRuDate } from '../../../utils'

type Props = {
  mode: TableFilterType,
  onOpenRemoveModal: (selectedEngine: RegistryRow) => void
}
export const getColumnsConfig = ({
  mode,
  onOpenRemoveModal
}: Props): ColumnsType<RegistryRow> => {

  return [
    {
      title: 'Наименование',
      dataIndex: 'name',
      width: mode === 'FILE' ? '87%' : '67%',
      render(_, record) {
        return <div className={styles.textWithWrap}>{record.name}</div>
      },
    },
    {
      title: 'Статус',
      dataIndex: 'status',
      hidden: mode === 'FILE',
      width: '10%',
      render(_, record) {
        let lable = ''
        let status: BadgePropStatus | undefined = undefined
        if (record.status === 'new') {
          lable = "Новый"
          status = 'normal'
        }
        if (record.status === 'pending') {
          lable = "В процессе"
          status = 'warning'
        }
        if (record.status === 'failed') {
          lable = "Ошибка"
          status = 'error'
        }
        if (record.status === 'succses') {
          lable = "Успешно обработан"
          status = 'success'
        }

        return <Badge size="s" label={lable} view="stroked" status={status} />;
      },
    },
    {
      title: 'Дата последнего анализа',
      dataIndex: 'lastAnalazeDate',
      hidden: mode === 'FILE',
      width: '10%',
      render(_, record) {
        const config: Intl.DateTimeFormatOptions = {
          day: 'numeric',
          month: 'numeric',
          year: 'numeric',
          hour: '2-digit',
          minute: '2-digit'
        }
        return <div className={styles.textWithWrap}>{getRuDate(record.lastAnalazeDate, config)}</div>
      }
    },
    {
      title: 'Наличие дефектов',
      dataIndex: 'isLastAnalyseHasDefect',
      width: '10%',
      render(_, record) {
        const text = record.isLastAnalyseHasDefect ? "Есть" : "Нет";
        const status = record.isLastAnalyseHasDefect ? "warning" : "success";
        return <Badge size="s" label={text} status={status} view="stroked" />;
      },
    },
    {
      title: '',
      dataIndex: 'actions',
      width: '3%',
      render(_, record) {
        return (
          <ContextButton
            size="xs"
            items={CONTEXT_ITEMS}
            getOnClick={(item) => {
              if (item.key === CONTEXT_ACTION.remove) { 
               onOpenRemoveModal(record)
              }
            }}
          />
        )
      },
    },
  ]
}
