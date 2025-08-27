import React from 'react'
import { Text } from '@consta/uikit/Text'

import { ReportCard } from '../../../components'
import { History } from '../..'

import styles from './styles.css'

export const Analytics = () => {

  return (
    <div className={styles.container}>
      <div className={styles.containerColumn}>
        <Text size="l" weight='semibold' className='m-4'>
          Диаграмма развития вероятности дефектов
        </Text>
        <ReportCard />
      </div>
      <div className={styles.graphics}>
        <Text size="l" weight='semibold' className='m-4'>
          График развития вероятности дефекта 1
        </Text>
        <History />
      </div>
    </div>
  )
}
