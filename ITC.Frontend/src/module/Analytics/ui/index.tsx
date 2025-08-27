import React from 'react'
import { Text } from '@consta/uikit/Text'
import { ResponsesSuccess } from '@consta/uikit/ResponsesSuccess';
import { ReportCard } from '../../../components'
import { History } from '../..'

import styles from './styles.css'
import { useParams } from 'react-router-dom'
import { EngineApi } from '../../../apiRTK'
import { Loader } from '@consta/uikit/Loader';

export const Analytics = () => {
  const { id = '' } = useParams()
  const { data } = EngineApi.useGetByIdQuery(id, { skip: !id })

  if (data?.status === 1) {
    return (
      <div className={styles.container}>
        <div className={styles.containerColumn}>
         <div className={styles.placeholder}>
            <Loader />
         </div>
        </div>
      </div>)
  }

  if (!data?.isLastAnalyseHasDefect && data?.status === 3) {
    return (
      <div className={styles.container}>
        <ResponsesSuccess className={styles.containerColumn} size="l" title="Дефекты не обнаружены!" actions={<></>} />
      </div>)
  }

  return (
    <div className={styles.container}>
      <div className={styles.containerColumn}>
        <Text size="l" weight='semibold' className='m-4'>
          Диаграмма развития степени дефектов
        </Text>
        <ReportCard />
      </div>
      <div className={styles.graphics}>
        <Text size="l" weight='semibold' className='m-4'>
          График развития степени дефекта
        </Text>
        <History />
      </div>
    </div>
  )
}
