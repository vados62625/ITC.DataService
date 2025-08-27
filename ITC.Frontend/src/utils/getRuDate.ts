/**
 * Функция приведения даты к RU формату
 * @example
 * // returns '01.01.2000'
 * getRuDate(new Date(2000,0,1,12,00))
 */
export const getRuDate = (date?: number | string | Date, config?: Intl.DateTimeFormatOptions) => {
  if (!date) {
    return ''
  }

  const formatter = new Intl.DateTimeFormat('ru-RU', config)
  return formatter.format(typeof date === 'string' ? new Date(date) : date)
}
