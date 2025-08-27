import React from 'react'
import type { TabsItemDefault, TabsProps } from '@consta/uikit/Tabs'
import { Tabs } from '@consta/uikit/Tabs'
import { IconDownload } from "@consta/uikit/IconDownload";

import { TABS } from '../constants'

import styles from './styles.css'
import { Toolbar } from '../../../components'
import { Button } from '@consta/uikit/Button'

type Props = {
  currentTab: TabsItemDefault
  onChangeTab: TabsProps['onChange']
  onDownload: () => void,
  isAnalyticTab: boolean
}

export const PageToolbar = ({
  currentTab,
  onChangeTab,
  onDownload,
  isAnalyticTab
}: Props) => {

  return (
    <Toolbar
      className={styles.toolbar}
      left={
        <Tabs
          size="s"
          view="clear"
          fitMode="scroll"
          items={TABS}
          value={currentTab}
          onChange={onChangeTab}
        />
      }
      right={
        isAnalyticTab && <Button
          label="Скачать"
          size="s"
          iconRight={IconDownload}
          onClick={onDownload}
        />
      }
    />
  )
}
