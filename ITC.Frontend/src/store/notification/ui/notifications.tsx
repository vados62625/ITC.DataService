import React from 'react'
import { useSelector } from 'react-redux'
import { SnackBar } from '@consta/uikit/SnackBar'
import { presetGpnDefault, Theme } from '@consta/uikit/Theme'

import { NotificationSlice } from '../model'

import styles from './styles.css'

export const Notifications = () => {
  const items = useSelector(NotificationSlice.selectors.notificationsSelector)

  return (
    <Theme preset={presetGpnDefault}>
      <div className={styles.notificationContainer}>
        <SnackBar className={styles.notifications} items={Object.values(items)} />
      </div>
    </Theme>
  )
}
