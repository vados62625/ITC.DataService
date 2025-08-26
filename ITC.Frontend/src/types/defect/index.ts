export const DefectName = {
  OUTER_RING : 'Дефект наружного кольца подшипника',
  INNER_RING : 'Дефект внутреннего кольца подшипника',
  ROLLING_ELEMENTS : 'Дефект тел качения',
  CAGE : 'Дефект сепаратора',
  UNBALANCE : 'Дисбаланс',
  MISALIGNMENT : 'Расцентровка'
} as const

export type DefectType = keyof typeof DefectName
export type DefectNameType = typeof DefectName[keyof typeof DefectName];

export type Defect = {
  type: DefectType;
  name: string;
  probability: number
}