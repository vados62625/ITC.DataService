import React from 'react'
import { Text } from '@consta/uikit/Text'
import cn from 'classnames'

import { TextWithLabel } from '../../../components'

import styles from './styles.css'

export const MainParams = () => {
  return (
    <div className={styles.container}>
      <div className={styles.containerColumn}>
        <Text size="l" weight="light">
          Параметры коммерческого условия
        </Text>
        <TextWithLabel
          label="Юридический номер"
          weight="semibold"
          text={'text'}
        />
        <TextWithLabel label="Период действия" weight="semibold" text={"period"} />
        <TextWithLabel label="ДО Исполнителя" weight="semibold" text={"contractorName"} />
        <TextWithLabel label="ДО Заказчика" weight="semibold" text={"customerNames"} />
        <div className={styles.row}>
          <TextWithLabel label="НДС" weight="semibold" text={"mainParams.nds"} />
          <TextWithLabel label="Стоимость c НДС" weight="semibold" text={"mainParams.costWithNds"} />
          <TextWithLabel
            label="Стоимость без НДС"
            weight="semibold"
            text={"mainParams.costWithoutNds"}
          />
        </div>
      </div>
    </div>
  )
}
