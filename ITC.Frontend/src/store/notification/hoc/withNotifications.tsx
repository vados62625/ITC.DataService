import React from 'react'
import { createPortal } from 'react-dom'

import { Notifications } from '../ui'

export const withNotifications = (AppComponent: React.ReactNode) => {
  const AppWithNotifications = () => (
    <>
      {AppComponent}
      {createPortal(<Notifications />, document.body)}
    </>
  )

  return AppWithNotifications
}
