import type { UnknownAction } from '@reduxjs/toolkit'
import type { BaseQueryFn } from '@reduxjs/toolkit/src/query/baseQueryTypes'
import type {
  FulfilledAction,
  MutationThunk,
  QueryThunk,
  RejectedAction,
} from '@reduxjs/toolkit/src/query/core/buildThunks'
import type { EndpointDefinition } from '@reduxjs/toolkit/src/query/endpointDefinitions'

export const ENTITIES = {
  engine: 'Двигатель',
} as const

export type Entity = (typeof ENTITIES)[keyof typeof ENTITIES]


 type apiActions =   | 'delete'
  | 'add'
  | 'update'
  | 'change'
  | 'addError'
  | 'updateError'
  | 'changeError'
  | 'deleteError'
  | 'saveError'
  | 'getError'
  | 'someError'
  | 'attachFile'
  | 'attachFileError'

type TQuery = QueryThunk | MutationThunk
type TEndpointDefinition = EndpointDefinition<unknown, BaseQueryFn, string, unknown>

type ApiActionMeta = {
  message?: string
  actionType?: apiActions
  entity?: Entity
  entityName?: string
}

export type TApiAction = (
  | (RejectedAction<TQuery, TEndpointDefinition> & UnknownAction)
  | (FulfilledAction<TQuery, TEndpointDefinition> & UnknownAction)
) & { meta: { baseQueryMeta?: ApiActionMeta } }
