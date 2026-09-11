import * as signalR from '@microsoft/signalr'
import { message } from 'antd'
import { useEffect, useRef, useState } from 'react'
import { api, getToken, type NotificationItem } from '../api'

// Хук: подключается к SignalR-хабу уведомлений, показывает live-toast и
// возвращает список уведомлений пользователя.
export function useNotifications(userId: number | null, enabled: boolean) {
  const [notifications, setNotifications] = useState<NotificationItem[]>([])
  const connectionRef = useRef<signalR.HubConnection | null>(null)
  const [connected, setConnected] = useState(false)

  const load = async () => {
    if (userId == null) return
    try {
      const items = await api.notificationsAll(userId)
      setNotifications(items)
    } catch {
      /* ignore */
    }
  }

  useEffect(() => {
    void load()
  }, [userId])

  useEffect(() => {
    if (!enabled || userId == null) return
    const token = getToken()
    if (!token) return

    const connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/notifications', { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build()

    connection.on('notification', (item: NotificationItem) => {
      setNotifications((prev) => [item, ...prev])
      let text = item.type
      try {
        const parsed = JSON.parse(item.payloadJson)
        if (parsed?.text) text = parsed.text
        else if (parsed?.title) text = parsed.title
      } catch {
        /* keep type */
      }
      void message.success(text || 'Новое уведомление')
    })

    connection
      .start()
      .then(() => setConnected(true))
      .catch(() => {
        /* offline — работаем по REST */
      })

    connectionRef.current = connection

    return () => {
      void connection.stop()
      connectionRef.current = null
      setConnected(false)
    }
  }, [enabled, userId])

  const markRead = async (id: number) => {
    setNotifications((prev) => prev.map((n) => (n.id === id ? { ...n, read: true } : n)))
    try {
      await api.markNotificationRead(id)
    } catch {
      /* ignore */
    }
  }

  return { notifications, connected, markRead, load }
}