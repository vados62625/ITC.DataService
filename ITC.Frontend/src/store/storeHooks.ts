import type { TypedUseSelectorHook } from 'react-redux'
import { useDispatch, useSelector } from 'react-redux'
import type { ThunkDispatch, UnknownAction } from '@reduxjs/toolkit'
import { RootState } from './store'

export const useAppDispatch: () => ThunkDispatch<RootState, unknown, UnknownAction> = useDispatch
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector
