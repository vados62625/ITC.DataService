import type { TRecord } from '../model'

type TRowEvents<T> = {
  onClick: (record: T) => void
}

const IGNORE_CLASSES = ['ListItemGrid-Slot', 'ListBox', 'ListItem', 'icons--Icon-Svg']

export const getRowEvents = <T extends TRecord>(record: T, { onClick }: TRowEvents<T>) => {
  const onClickRow = (e: React.MouseEvent) => {
    const target = e.target as HTMLElement
    const targetClasses = [...target.classList]
    const nodeName = target.nodeName
    const isTargetHasIgnoreClasses = Boolean(
      targetClasses.filter(cn => IGNORE_CLASSES.includes(cn)).length,
    )

    // Необходима проверка на что кликнули, так как antd table может вызвать onCLick при клике на контекстное меню
    if (!('id' in record) || isTargetHasIgnoreClasses || nodeName === 'path') {
      return
    }
    onClick(record)
  }

  return {
    onClick: onClickRow,
  }
}
