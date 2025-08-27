import { IconDownload } from '@consta/icons/IconDownload'
import { IconTrash } from '@consta/icons/IconTrash'
import type { ContextMenuItemDefault } from '@consta/uikit/ContextMenu'

export const CONTEXT_ACTION = {
  report: 'report',
  remove: 'remove',
} as const

export type ContextItemType = ContextMenuItemDefault & {
  key: keyof typeof CONTEXT_ACTION
}

export const CONTEXT_ITEMS: ContextItemType[] = [
  {
    label: 'Удалить',
    leftIcon: IconTrash,
    key: CONTEXT_ACTION.remove,
  },
]
