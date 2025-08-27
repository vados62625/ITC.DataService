import React, { useEffect } from 'react'
import { Table } from '../../../components'

import { getColumnsConfig } from '../config'

import styles from './styles.css'
import { RegistryRow, RoutePaths } from '../../../types'
import { useNavigate } from 'react-router-dom'
import { RegistrySlice } from '../../../store'
import { useDispatch, useSelector } from 'react-redux'
import { EngineApi } from '../../../apiRTK'

export const EnginesTable = () => {
  const mode = useSelector(RegistrySlice.selectors.mode)
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const [lazyGetByFilter, { data, isLoading } ]= EngineApi.useLazyGetAllByFilterQuery()

  useEffect(() => {
    const interval = setInterval(() => {
      lazyGetByFilter(mode).refetch()
    }, 3000);

    return () => clearTimeout(interval);
  }, [mode, lazyGetByFilter]);
  
  const onRowClick = (record: RegistryRow) => {
    navigate(`${RoutePaths.Details}/${record.id}`);
  }

  const onOpenRemoveModal = (selectedEngine: RegistryRow) => {
    dispatch(RegistrySlice.actions.changeRemoveModal({ isOpen: true, selectedEngine }))
  };

  const columnsConfig = getColumnsConfig({ mode, onOpenRemoveModal })

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
