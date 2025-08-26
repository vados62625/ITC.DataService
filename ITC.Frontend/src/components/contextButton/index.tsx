import React, { useRef, useState } from 'react'
import type { IconComponent } from '@consta/icons/Icon'
import { IconKebab } from '@consta/icons/IconKebab'
import { IconSelect } from '@consta/icons/IconSelect'
import type { ButtonPropForm, ButtonPropView, Props } from '@consta/uikit/Button'
import { Button } from '@consta/uikit/Button'
import type {
  ContextMenuItemDefault,
  ContextMenuPropGetItemStatus,
  ContextMenuPropOnClick,
} from '@consta/uikit/ContextMenu'
import { ContextMenu } from '@consta/uikit/ContextMenu'

type TMenuActions<T extends ContextMenuItemDefault> = {
  isDisabled?: boolean
  isLoading?: boolean
  direction?: React.ComponentProps<typeof ContextMenu>['direction']
  items: T[]
  buttonView?: ButtonPropView
  form?: ButtonPropForm
  className?: string
  size?: Props['size']
  groups?: { name?: string; key: number }[]
  getOnClick: (item: T) => void
  getItemDisabled?: (item: T) => boolean
  icon?: IconComponent
  getItemStatus?: (item: T) => 'normal' | undefined
  label?: string
  iconRight?: IconComponent
}

export const ContextButton = <T extends ContextMenuItemDefault>({
  isDisabled,
  isLoading,
  items,
  direction = 'rightStartUp',
  buttonView = 'clear',
  className,
  size = 's',
  groups,
  form,
  getItemStatus,
  getOnClick,
  getItemDisabled,
  icon = IconKebab,
  iconRight = IconSelect,
  label,
}: TMenuActions<T>) => {
  const [isMenuActive, setIsMenuActive] = useState(false)
  const buttonRef = useRef(null)

  const switchMenuActive = (event: React.MouseEvent<HTMLElement>) => {
    event.stopPropagation()
    event.preventDefault()
    setIsMenuActive(prevState => !prevState)
  }


  const onContextClick = ():ContextMenuPropOnClick<T> | undefined => {
    return ({e, item}) => {
      e.stopPropagation()
      getOnClick(item)
      setIsMenuActive(false)
    }
  }

  const isContextMenuActive = isMenuActive && items.length > 0

  return (
    <>
      <Button
        ref={buttonRef}
        className={className}
        onlyIcon={!label}
        label={label}
        size={size}
        iconLeft={icon}
        iconRight={label ? iconRight : undefined}
        view={buttonView}
        loading={isLoading}
        disabled={isDisabled}
        data-testid="contextButton"
        form={form}
        onClick={switchMenuActive}
      />

      <ContextMenu
        size="s"
        isOpen={isContextMenuActive}
        items={items}
        getItemLabel={item => item.label}
        getItemLeftIcon={item => item.leftIcon}
        anchorRef={buttonRef}
        getItemDisabled={item => item.disabled || getItemDisabled?.(item)}
        direction={direction}
        groups={groups}
        getItemStatus={getItemStatus as ContextMenuPropGetItemStatus<T>}
        getGroupId={group => group.key}
        getGroupLabel={group => group.name}
        getItemOnClick={onContextClick}
        data-testid="contextMenu"
        onClickOutside={() => {
          setIsMenuActive(false)
        }}
      />
    </>
  )
}
