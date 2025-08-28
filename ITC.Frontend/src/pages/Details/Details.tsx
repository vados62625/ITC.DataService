import React, { useState } from 'react'

import { useParams } from 'react-router-dom'
import { Loader } from '@consta/uikit/Loader'
import type { TabsItemDefault } from '@consta/uikit/Tabs'
import { TABS, PageToolbar } from '../../components'

import styles from './styles.css'
import { Analytics, MainParams } from '../../module'
import { EngineApi } from '../../apiRTK'

export const Details = () => {
  const [currentTabs, setCurrentTabs] = useState<TabsItemDefault>(TABS[0])
  const { id = '' } = useParams()
  const { data, isLoading } = EngineApi.useGetByIdQuery(id, { skip: !id, refetchOnMountOrArgChange: true })

  const isMainParamsTab = currentTabs === TABS[0]
  const isAnalyticTab = currentTabs === TABS[1]

  const onDownload = () => {
    window.print()
  }

  if (isLoading) {
    return <Loader className="loaderFullContent" />
  }

  return (
    <div className={styles.container}>
      <PageToolbar
        currentTab={currentTabs}
        isAnalyticTab={isAnalyticTab}
        isDownloadButtonVisible={!!data?.isLastAnalyseHasDefect}
        onChangeTab={({ value }) => {
          setCurrentTabs(value)
        }}
        onDownload={onDownload}
      />
      <div className={styles.tabContent}>
        {isMainParamsTab && <MainParams />}
        {isAnalyticTab && <Analytics />}
      </div>
    </div>
  )
}