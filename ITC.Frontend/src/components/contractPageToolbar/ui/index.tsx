import React from 'react'
import type { TabsItemDefault, TabsProps } from '@consta/uikit/Tabs'
import { Tabs } from '@consta/uikit/Tabs'

import { CONTRACT_TABS } from '../constants'

import styles from './styles.css'
import { Toolbar } from '../../../components'

type Props = {
  currentTab: TabsItemDefault
  onChangeTab: TabsProps['onChange']
}

export const ContractPageToolbar = ({
  currentTab,
  onChangeTab,
}: Props) => {

  return (
    <Toolbar
      className={styles.toolbar}
      left={
        <Tabs
          size="s"
          view="clear"
          fitMode="scroll"
          items={CONTRACT_TABS}
          value={currentTab}
          onChange={onChangeTab}
        />
      }
    />
  )
}
