export type PageTab = {
  key: TabKey
  label: string
  path: string
}

type TabKey = 'info' | 'analytics'

export const CONTRACT_TABS: PageTab[] = [
  {
    key: 'info',
    label: 'Общая информация',
    path: ""
  },
  {
    key: 'analytics',
    label: 'Аналитика',
    path: ""
  },
]
