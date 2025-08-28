import type React from 'react'

import { useAddSignalRHub } from '../hooks'
import { EngineHub } from '../../apiRTK/engine/signalR'

type TSignalRProvider = {
  children: React.ReactNode
}

export const SignalRProvider = ({ children }: TSignalRProvider) => {
  useAddSignalRHub({ hubName: EngineHub.hubUrl, hubMethods: EngineHub.events })

  return children
}
