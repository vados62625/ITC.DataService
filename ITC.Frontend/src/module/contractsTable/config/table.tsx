import React from 'react'
import type { ColumnsType } from 'antd/es/table'

import { CONTEXT_ITEMS } from './contextButton'
import { Badge, BadgePropStatus } from '@consta/uikit/Badge'

import styles from './styles.css'
import { RegistryRow } from 'src/types'
import { ContextButton } from '../../../components'

type Props = {
  onContextClick: (record: RegistryRow) => void
}

export const getColumnsConfig = ({
  onContextClick,
}: Props): ColumnsType<RegistryRow> => [
    {
      title: 'Наименование',
      dataIndex: 'name',
      width: 550,
      render(value) {
        return <div className={styles.textWithWrap}>{value}</div>
      },
    },
     {
      title: 'Статус',
      dataIndex: 'status',
      width: 100,
      render(value) {
        let lable = ''
        let status: BadgePropStatus | undefined = undefined
        if (value === 'new') {
          lable = "Новый"
          status = 'normal'
        }
        if (value === 'pending') {
          lable = "В процессе"
          status = 'warning'
        }
        if (value === 'failed') {
          lable = "Ошибка"
          status = 'error'
        }
        if (value === 'succses') {
          lable = "Успешно обработан"
          status = 'success'
        }


        return <Badge size="s" label={lable} view="stroked" status={status} />;
      },
    },
    {
      title: 'Наличие дефектов',
      dataIndex: 'isHasDefect',
      width: 50,
      render(value) {
        const text = value ? "Есть" : "Нет";
        const status = value ? "warning" : "success";
        return <Badge size="s" label={text} status={status} view="stroked" />;
      },
    },
    {
      title: 'Дата последнего анализа',
      dataIndex: 'lastAnalazeDate',
      width: 100,
      render(value) {
        return <div className={styles.textWithWrap}>{value}</div>
      },
    },
    {
      title: '',
      dataIndex: 'actions',
      width: 25,
      render(_, record) {
        return (
          <ContextButton
            size="xs"
            items={CONTEXT_ITEMS}
            getOnClick={() => {
              onContextClick(record)
            }}
          />
        )
      },
    },
  ]