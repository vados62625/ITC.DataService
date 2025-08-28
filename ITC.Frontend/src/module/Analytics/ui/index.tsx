import React, { ReactNode } from 'react'
import { Text } from '@consta/uikit/Text'
import { ResponsesSuccess } from '@consta/uikit/ResponsesSuccess';
import { Responses500 } from '@consta/uikit/Responses500';
import { ReportCard } from '../../../components'
import { History } from '../..'

import styles from './styles.css'
import { useParams } from 'react-router-dom'
import { EngineApi } from '../../../apiRTK'
import { Loader } from '@consta/uikit/Loader';

type StatusContainerProps = {
  children: ReactNode
}

const StatusContainer = ({ children }: StatusContainerProps) => (
  <div className={styles.container}>{children}</div>
)

export const Analytics = () => {
  const { id = '' } = useParams()
  const { data } = EngineApi.useGetByIdQuery(id, { skip: !id, refetchOnMountOrArgChange: true })

  switch (data?.engineStatus) {
    case 1:
      return (
        <StatusContainer>
          <div className={styles.containerColumn}>
            <div className={styles.placeholder}>
              <Loader />
            </div>
          </div>
        </StatusContainer>
      )

    case 2:
      return (
        <StatusContainer>
          <Responses500 className={styles.containerColumn} size="l" actions={<></>}/>
        </StatusContainer>
      )

    case 3:
      return data?.isLastAnalyseHasDefect ? (
        <MainContent />
      ) : (
        <StatusContainer>
          <ResponsesSuccess
            className={styles.containerColumn}
            size="l"
            title="Дефекты не обнаружены!"
            actions={<></>}
          />
        </StatusContainer>
      )

    default:
      return <MainContent />
  }
}

const MainContent = () => (
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