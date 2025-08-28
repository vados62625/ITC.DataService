export const isDateStringGuard = (value: unknown): value is string => {
  return /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d+Z/.test(value as string)
}

export const isObjectGuard = (value: unknown): value is Record<string, unknown> => {
  return typeof value === 'object'
}

export const isArrayGuard = (value: unknown): value is unknown[] => {
  return Array.isArray(value)
}
