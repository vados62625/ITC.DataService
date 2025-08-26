import React from 'react'
import cn from 'classnames'

import styles from './styles.css'

type TToolbar = {
  left?: React.ReactNode
  right?: React.ReactNode
  center?: React.ReactNode
  className?: string
  style?: React.CSSProperties
}
export const Toolbar = ({ left, right, center, className, style }: TToolbar) => {
  return (
    <div
      data-testid="toolbar"
      className={cn(styles.container, className)}
      style={style}>
      {left && (
        <div data-testid="toolbar_left" className={cn(styles.item, styles.left)}>
          {left}
        </div>
      )}
      {center && (
        <div data-testid="toolbar_center" className={cn(styles.item, styles.center)}>
          {center}
        </div>
      )}
      {right && (
        <div data-testid="toolbar_right" className={cn(styles.item, styles.right)}>
          {right}
        </div>
      )}
    </div>
  )
}
