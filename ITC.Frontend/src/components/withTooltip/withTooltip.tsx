import React, { useEffect, useRef } from 'react'
import { Text } from '@consta/uikit/Text'
import { Tooltip } from '@consta/uikit/Tooltip'
import { useFlag } from '@consta/uikit/useFlag'
import { useForkRef } from '@consta/uikit/useForkRef'

type WithTooltipProps = React.ComponentProps<typeof Tooltip> & {
  children: React.ReactElement
  /** Текст для отображения внутри Tooltip */
  text: React.ReactNode
  /** Свойство отключения Tooltip */
  disabled?: boolean
  mode?: 'click' | 'mouseover'
  delay?: number
}

type Timer = ReturnType<typeof setTimeout>

/**
 * Компонент обертка для использования Tooltip в виде:
 *
 * @example
 * <WithTooltip>
 *   <div>children</div>
 * </WithTooltip>
 */
export const WithTooltip = ({
  children,
  text,
  disabled,
  mode = 'mouseover',
  delay = 0,
  ...tooltipProps
}: WithTooltipProps) => {
  const childrenRef = useRef(null)
  const timerRef = useRef<Timer | null>(null)
  const [isTooltipVisible, setIsTooltipVisible] = useFlag()

  // eslint-disable-next-line @typescript-eslint/no-unsafe-argument, @typescript-eslint/no-unsafe-member-access, @typescript-eslint/no-explicit-any
  const mergedRef = useForkRef([(children as any)?.ref, childrenRef])

  useEffect(() => {
    return () => {
      timerRef.current && clearTimeout(timerRef.current)
    }
  }, [])

  return (
    <>
      {React.cloneElement(children, {
        ref: mergedRef,
        onMouseOver: (e: MouseEvent) => {
          // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access,@typescript-eslint/no-unsafe-call
          children.props.onMouseOver && children.props.onMouseOver(e)
          if (mode === 'mouseover' && !disabled) {
            timerRef.current = setTimeout(setIsTooltipVisible.on, delay)
          }
        },
        onMouseOut: (e: MouseEvent) => {
          // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access,@typescript-eslint/no-unsafe-call
          children.props.onMouseOut && children.props.onMouseOut(e)
          if (mode === 'mouseover' && !disabled) {
            setIsTooltipVisible.off()
            timerRef.current && clearTimeout(timerRef.current)
          }
        },
        onClick: (e: MouseEvent) => {
          // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access,@typescript-eslint/no-unsafe-call
          children.props.onClick && children.props.onClick(e)
          if (mode === 'click' && !disabled) {
            setIsTooltipVisible.toggle()
          }
        },
      })}
      <Tooltip anchorRef={childrenRef} 
      // isOpen={isTooltipVisible} 
      {...tooltipProps}>
        {typeof text === 'string' ? (
          <Text size="s" view="primary">
            {text}
          </Text>
        ) : (
          text
        )}
      </Tooltip>
    </>
  )
}
