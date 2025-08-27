import React from 'react'
import { Table } from '../../../components'


import type { ContextItemType } from '../config'
import { CONTEXT_ACTION, getColumnsConfig } from '../config'

import styles from './styles.css'
import { RegistryRow, RoutePaths } from '../../../types'
import { useNavigate } from 'react-router-dom'
import { RegistrySlice } from '../../../store'
import { useSelector } from 'react-redux'
import { EngineApi } from '../../../apiRTK'

export const EnginesTable = () => {
  const mode = useSelector(RegistrySlice.selectors.mode)
  const navigate = useNavigate();

  const { data, isLoading } = EngineApi.useGetAllByFilterQuery(mode)
  const [deleteEngine] = EngineApi.useDeleteMutation()

  const removeEngine = (id: string) => {
    deleteEngine(id)
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
        removeEngine(record.id)
        break
      default:
        break
    }
  }

  const columnsConfig = getColumnsConfig({ onContextClick, mode })

  return (
    <div className={styles.tableContainer}>
      <Table
        columns={columnsConfig}
        rowClassName={styles.row}
        dataSource={data}
        loading={isLoading}
        virtual={false}
        onRowClick={onRowClick}
      />
    </div>
  )
}
