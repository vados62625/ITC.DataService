import { useEffect } from 'react'
import { useDispatch } from 'react-redux'
import { createAction } from '@reduxjs/toolkit'
import { SignalRTypes } from '../../store'
import { signalREvents } from '../../store/signalR/libs/guards'

type TUseAddSignalRHub = {
  hubName: string
  hubMethods: SignalRTypes.THubMethods
}

/**
 * Регистрирует SignalR подписку
 * @param hubSettings
 */
export const useAddSignalRHub = (hubSettings: TUseAddSignalRHub) => {
  const dispatch = useDispatch()
  useEffect(() => {
    const startConnectionAction = createAction(signalREvents.start, () => ({
      payload: {
        hubUrl: hubSettings.hubName,
        hubMethods: hubSettings.hubMethods,
      },
    }))
    dispatch(startConnectionAction())

    return () => {
      const stopConnectionAction = createAction(signalREvents.stop, () => ({
        payload: {
          hubUrl: hubSettings.hubName,
        },
      }))
      dispatch(stopConnectionAction())
    }
  }, [])
}
