import { Badge, List, Typography, Tag, Button, Empty } from 'antd'
import { ReloadOutlined } from '@ant-design/icons'
import { useAuth } from '../auth'
import { useNotifications } from '../hooks/useNotifications'

function displayText(item: { type: string; payloadJson: string }): string {
  try {
    const parsed = JSON.parse(item.payloadJson)
    if (parsed?.text) return parsed.text
    if (parsed?.title) return parsed.title
  } catch {
    /* ignore */
  }
  return item.type
}

export default function NotificationsPage() {
  const { user } = useAuth()
  const { notifications, connected, markRead, load } = useNotifications(user?.id ?? null, true)

  const unread = notifications.filter((n) => !n.read).length

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <Typography.Title level={4} style={{ margin: 0 }}>
          Уведомления <Badge count={unread} overflowCount={99} />
        </Typography.Title>
        <div>
          <Tag color={connected ? 'green' : 'default'}>{connected ? 'SignalR онлайн' : 'оффлайн'}</Tag>
          <Button icon={<ReloadOutlined />} onClick={() => void load()} style={{ marginLeft: 8 }}>
            Обновить
          </Button>
        </div>
      </div>

      {notifications.length === 0 ? (
        <Empty description="Уведомлений пока нет" />
      ) : (
        <List
          dataSource={notifications}
          renderItem={(item) => (
            <List.Item
              style={item.read ? { opacity: 0.55 } : undefined}
              actions={
                item.read
                  ? []
                  : [
                      <Button size="small" key="read" onClick={() => void markRead(item.id)}>
                        Прочитано
                      </Button>,
                    ]
              }
            >
              <List.Item.Meta
                title={
                  <span>
                    <Tag>{item.type}</Tag>
                    {new Date(item.createdAt).toLocaleString('ru-RU')}
                  </span>
                }
                description={displayText(item)}
              />
            </List.Item>
          )}
        />
      )}
    </div>
  )
}