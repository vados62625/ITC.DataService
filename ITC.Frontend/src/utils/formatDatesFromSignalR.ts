import { isArrayGuard, isDateStringGuard, isObjectGuard } from './baseGuards'

export const formatDatesFromSignalR = (data: unknown) => {
  try {
    if (isArrayGuard(data)) {
      return formatArray(data)
    }

    if (isObjectGuard(data)) {
      return formatObject(data)
    }

    if (isDateStringGuard(data)) {
      return formatDate(data)
    }

    return data
  } catch (e) {
    console.error('parse signalR data error:', e)
  }
}

export const formatDate = (value: string) => {
  return new Date(value.slice(0, -1))
}

export const formatObject = (value: Record<string, unknown>) => {
  for (const key in value) {
    if (isArrayGuard(value[key])) {
      value[key] = formatArray(value[key])
      continue
    }

    if (isObjectGuard(value[key])) {
      value[key] = formatObject(value[key])
      continue
    }

    if (isDateStringGuard(value[key])) {
      value[key] = formatDate(value[key])
    }
  }

  return value
}

export const formatArray = (value: unknown[]) => {
  return value.map(el => {
    if (isObjectGuard(el)) {
      return formatObject(el)
    }

    if (isDateStringGuard(el)) {
      return formatDate(el)
    }

    return el
  })
}
