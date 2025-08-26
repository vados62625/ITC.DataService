import React from 'react'
import { Table } from '../../../components/table'

import type { ContextItemType } from '../config'
import { CONTEXT_ACTION, getColumnsConfig } from '../config'

import styles from './styles.css'
import { RegistryRow, RoutePaths } from '../../../types'
import { useNavigate } from 'react-router-dom'

export const ContractsTable = () => {
  const navigate = useNavigate();

  const isLoading = false
const dataSource: RegistryRow[] = [
  {
    id: '1',
    name: 'Асинхронный двигатель АИР80В4',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '2',
    name: 'Электродвигатель 5АМ112МВ6',
    isHasDefect: true,
    status: 'new',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '3',
    name: 'Двигатель 4АМ90L2',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '4',
    name: 'Мотор АИР71А2',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '5',
    name: 'Электродвигатель ВАО72-4',
    isHasDefect: true,
    status: 'pending',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '6',
    name: 'АИР100S4',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '7',
    name: 'Двигатель 5АМ132S8',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '8',
    name: 'Мотор АИР56В6',
    isHasDefect: true,
    status: 'pending',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '9',
    name: 'Электродвигатель 4АМ80А4',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '10',
    name: 'АИР63В2',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '11',
    name: 'Двигатель 5АМ160М4',
    isHasDefect: true,
    status: 'pending',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '12',
    name: 'Мотор ВАО82-6',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '13',
    name: 'Электродвигатель АИР112МВ8',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '14',
    name: 'АИР75С4',
    isHasDefect: true,
    status: 'pending',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '15',
    name: 'Двигатель 4АМ132М2',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '16',
    name: 'Мотор 5АМ90L6',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '17',
    name: 'Электродвигатель АИР80А4',
    isHasDefect: true,
    status: 'pending',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '18',
    name: 'ВАО63-2',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '19',
    name: 'Двигатель 4АМ112МА6',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '20',
    name: 'Мотор АИР71В4',
    isHasDefect: true,
    status: 'pending',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '21',
    name: 'Электродвигатель 5АМ100L8',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '22',
    name: 'АИР56А2',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '23',
    name: 'Двигатель ВАО72-6',
    isHasDefect: true,
    status: 'pending',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '24',
    name: 'Мотор 4АМ160S4',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '25',
    name: 'Электродвигатель АИР132МА2',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '26',
    name: 'АИР80В6',
    isHasDefect: true,
    status: 'pending',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '27',
    name: 'Двигатель 5АМ112S4',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '28',
    name: 'Мотор ВАО82-4',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '29',
    name: 'Электродвигатель АИР90L8',
    isHasDefect: true,
    status: 'pending',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '30',
    name: '4АМ75В2',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '31',
    name: 'Двигатель АИР63С4',
    isHasDefect: true,
    status: 'failed',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '32',
    name: 'Мотор 5АМ132М6',
    isHasDefect: true,
    status: 'pending',
    lastAnalazeDate: '23.08.2025'
  },
  {
    id: '33',
    name: 'Электродвигатель ВАО71-8',
    isHasDefect: false,
    status: 'succses',
    lastAnalazeDate: '23.08.2025'
  }
]

  const remove = (id: string) => {
  }

  const download = (id: string) => {

  }

  const onRowClick = (record: RegistryRow) => {
    navigate(`${RoutePaths.Details}/${record.id}`);
  }

  const onContextClick = (record: RegistryRow) => (item: ContextItemType) => {
    if (!record.id) {
      return
    }

    switch (item.key) {
      case CONTEXT_ACTION.report:
        download(record.id)
        break
      case CONTEXT_ACTION.remove:
        remove(record.id)
        break
      default:
        break
    }
  }

  const columnsConfig = getColumnsConfig({ onContextClick })

  return (
    <div className={styles.tableContainer}>
      <Table
        columns={columnsConfig}
        rowClassName={styles.row}
        dataSource={dataSource}
        loading={isLoading}
        virtual={false}
        onRowClick={onRowClick}
      />
    </div>
  )
}
