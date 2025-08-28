import React from 'react'
import { Text } from '@consta/uikit/Text'

import { TextWithLabel } from '../../../components'

import styles from './styles.css'
import { useParams } from 'react-router-dom'
import { EngineApi } from '../../../apiRTK'
import { Loader } from '@consta/uikit/Loader'

export const MainParams = () => {
  const { id = '' } = useParams()
  const { data, isLoading } = EngineApi.useGetByIdQuery(id, { skip: !id, refetchOnMountOrArgChange: true })

  if (isLoading) {
    return <Loader style={{ height: "100%", width: "100%" }} />
  }

  return (
    <div className={styles.container}>
      <div className={styles.containerColumn}>
        <Text size="l" weight="light">
          Параметры стенда
        </Text>
        <TextWithLabel
          label="Номер двигателя"
          weight="semibold"
          text={data?.name}
        />
        <TextWithLabel label="Тип ЭД" weight="semibold" text={"Асинхронный"} />
        <TextWithLabel label="Номинальная мощность ЭД, кВт" weight="semibold" text={"3"} />
        <TextWithLabel label="Подшипники" weight="semibold" text={"NSK6205DDU"} />

        <TextWithLabel label="Номинальная частота вращения ЭД, об/мин" weight="semibold" text={"1770"} />
        <TextWithLabel label="Номинальная частота вращения выходного вала мультипликатора, об/мин" weight="semibold" text={"3010"} />
        <TextWithLabel
          label="Частота дискретизации сигналов тока потребления ЭД, кГц"
          weight="semibold"
          text={"25,6"}
        />
      </div>
    </div>
  )
}
