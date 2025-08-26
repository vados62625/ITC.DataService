import React from 'react'
import type { TextPropWeight } from '@consta/uikit/Text'
import { Text } from '@consta/uikit/Text'
import { withTooltip } from '@consta/uikit/withTooltip'

type TTextWithLabel = {
  label: string
  text?: React.ReactNode
  truncate?: boolean
  className?: string
  isTooltipEnabled?: boolean
  weight?: TextPropWeight
}

const TextWithTooltip = withTooltip({ content: '' })(Text)

export const TextWithLabel = ({
  label,
  text,
  className,
  truncate = false,
  isTooltipEnabled,
  weight,
}: TTextWithLabel) => {
  const isTextComponent = typeof text === 'string'

  return (
    <div className={className} data-testid="textWithLabel">
      <Text size="xs" view="secondary" lineHeight="xs">
        {label}
      </Text>
      <TextWithTooltip
        size="s"
        lineHeight="m"
        view="primary"
        weight={weight}
        truncate={truncate}
        tooltipProps={{
          // tooltipContent: !isTextComponent || !isTooltipEnabled ? undefined : text,
          direction: 'upStartLeft',
        }}>
        {text ? text : <>&mdash;</>}
      </TextWithTooltip>
    </div>
  )
}
