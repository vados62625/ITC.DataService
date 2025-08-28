import React from 'react'
import type { IconComponent, IconPropSize, IconPropView } from '@consta/icons/Icon'
import type { Direction } from '@consta/uikit/Popover'
import { Text } from '@consta/uikit/Text'
import type { TooltipPropSize } from '@consta/uikit/Tooltip'
import { WithTooltip } from '../withTooltip'

type IconWithTooltipProps = {
  Icon: IconComponent
  tooltip: string
  direction?: Direction
  size?: IconPropSize
  view?: IconPropView
  disabled?: boolean
  tooltipSize?: TooltipPropSize
}

export const IconWithTooltip = ({
  Icon,
  tooltip,
  direction = 'upStartLeft',
  size = 's',
  view = 'secondary',
  disabled,
  tooltipSize,
}: IconWithTooltipProps) => {
  return (
    <WithTooltip
      style={{ zIndex: 1000 }}
      direction={direction}
      disabled={disabled}
      size={tooltipSize}
      placeholder={undefined}
      onPointerEnterCapture={undefined}
      onPointerLeaveCapture={undefined}
      text={<Text view="primary" lineHeight="m" size="xs" style={{ whiteSpace: 'pre-line' }}>
        {tooltip}
      </Text>} >
      <Icon className="m-l-2xs" size={size} view={view} />
    </WithTooltip>
  )
}
