export type PageTab = {
  key: TabKey
  label: string
}

type TabKey = 'info' | 'analytics'

export const TABS: PageTab[] = [
  {
    key: 'info',
    label: 'Общая информация',
  },
  {
    key: 'analytics',
    label: 'Аналитика',
  },
]
