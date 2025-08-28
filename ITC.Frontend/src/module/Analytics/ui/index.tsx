import React, { ReactNode } from 'react'
import { Text } from '@consta/uikit/Text'
import { ResponsesSuccess } from '@consta/uikit/ResponsesSuccess';
import { Responses500 } from '@consta/uikit/Responses500';
import { ReportCard } from '../../../components'
import { History } from '../..'
import { ProgressSpin } from '@consta/uikit/ProgressSpin';
import styles from './styles.css'
import { useParams } from 'react-router-dom'
import { EngineApi } from '../../../apiRTK'
import { Informer } from '@consta/uikit/Informer';
import { IconWarning } from "@consta/uikit/IconWarning"
import { IconEye } from "@consta/uikit/IconEye"
import { IconAlert } from "@consta/uikit/IconAlert"
import cn from 'classnames';
import { withTooltip } from '../../../hocs';

type StatusContainerProps = {
  children: ReactNode
}

export const Analytics = () => {
  const { id = '' } = useParams()
  const { data } = EngineApi.useGetByIdQuery(id, { skip: !id, refetchOnMountOrArgChange: true })

  const isInformerVisible = !!data?.recommendedMaintenanceDate
  const recommendedMaintenanceDate = data?.recommendedMaintenanceDate?.toLocaleDateString() ?? ''

  switch (data?.engineStatus) {
    case 1:
      return (
        <div className="container-row w-100 justify-center align-center p-t-4">
          <div className={cn("container-row justify-center align-center", styles.containerColumn)}>
            <Text size="l" weight='semibold' className='m-4'>
              Выполняется анализ, пожалуйста подождите...
            </Text>
            <ProgressSpin size='2xl' />
          </div>
        </div>
      )

    case 2:
      return (
        <div className="container-row w-100 justify-center align-center p-t-4">
          <div className={cn("container-row justify-center align-center", styles.containerColumn)}>
            <Responses500 className={styles.containerColumn} size="l" actions={<></>} />
          </div>
        </div>
      )

    case 3:
      return (
        data?.isLastAnalyseHasDefect ? (
          <MainContent recommendedMaintenanceDate={recommendedMaintenanceDate} isInformerVisible={isInformerVisible} />
        ) : (
          <div className="container-row w-100 justify-center align-center p-t-4">
            <div className={cn("container-row justify-center align-center", styles.containerColumn)}>
              <ResponsesSuccess
                size="l"
                title="Дефекты не обнаружены!"
                actions={<></>}
              />
            </div>
          </div>
        )
      )

    default:
      return <MainContent recommendedMaintenanceDate={recommendedMaintenanceDate} isInformerVisible={isInformerVisible} />
  }
}

type Props = {
  recommendedMaintenanceDate: string
  isInformerVisible: boolean
}

const IconAlertWithTooltip = withTooltip(() => {
  return (
    <IconAlert size='s' />
  );
});


const IconEyeWithTooltip = withTooltip(() => {
  return (
    <IconEye size='s' />
  );
});


const IconWarningithTooltip = withTooltip(() => {
  return (
    <IconWarning size='s' />
  );
});

const MainContent = ({ recommendedMaintenanceDate, isInformerVisible }: Props) => (
  <div className={styles.container}>
    <div className={styles.analytics}>
      <div className={styles.containerColumn}>
        <Text size="l" weight='semibold' className='m-4'>
          Диаграмма степени развития дефектов
        </Text>
        <ReportCard />
        <div className="m-l-4 m-b-2 container-row">
          <div className={cn(styles.cercle, styles.normal)}>
            <IconEyeWithTooltip>
              <Text style={{ color: '#fff' }} size='s'>
                Низкая степень развития
              </Text>
            </IconEyeWithTooltip>
          </div>
          <div className={cn("m-l-2 m-r-2", styles.cercle, styles.warning)}>
            <IconAlertWithTooltip>
              <Text style={{ color: '#fff' }} size='s'>
                Высокая степень развития
              </Text>
            </IconAlertWithTooltip>
          </div>
          <div className={cn(styles.cercle, styles.alert)}>
            <IconWarningithTooltip>
              <Text style={{ color: '#fff' }} size='s'>
                Критическая степень развития
              </Text>
            </IconWarningithTooltip>
          </div>
        </div>
      </div>
      <div className={styles.graphics}>
        <Text size="l" weight='semibold' className='m-4'>
          График степени развития дефекта
        </Text>
        <History />
      </div>
    </div>
    {
      isInformerVisible && (
        <Informer
          className={styles.informer}
          status="warning"
          view="filled"
          icon={IconWarning}
          title="Внимание!"
          label={`Во избежание отказа двигателя рекомендуем провести его техническое обслуживание до ${recommendedMaintenanceDate}`}
        />
      )
    }
  </div>
)