import React, { useState } from 'react'
import { useDispatch } from 'react-redux'
import { useParams } from 'react-router-dom'
import { Loader } from '@consta/uikit/Loader'
import type { TabsItemDefault } from '@consta/uikit/Tabs'
import { CONTRACT_TABS, ContractPageToolbar } from '../../components'

import styles from './styles.css'
import { Diagram, MainParams } from '../../module'

export const Details = () => {
  const [currentTabs, setCurrentTabs] = useState<TabsItemDefault>(CONTRACT_TABS[0])
  const { id = '' } = useParams()

  const dispatch = useDispatch()

  const isMainParamsTab = currentTabs === CONTRACT_TABS[0]
  const isAnalyticTab = currentTabs === CONTRACT_TABS[1]
  const isLoading = false

  if (isLoading) {
    return <Loader className="loaderFullContent" />
  }

  return (
    <div className={styles.container}>
      <ContractPageToolbar
        currentTab={currentTabs}
        // isMainParamsTab={isMainParamsTab}
        // isAnalyticTab={isAnalyticTab}
        onChangeTab={({ value }) => {
          setCurrentTabs(value)
        }}
      />
      <div className={styles.tabContent}>
        {isMainParamsTab && <MainParams />}
        {isAnalyticTab && <Diagram />}
      </div>
    </div>
  )
}
