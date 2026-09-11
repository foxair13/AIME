import { useEffect, useState } from 'react'
import { Avatar, Card, List, Tag, Typography } from 'antd'
import { api, type User } from '../api'

export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
      try {
        setUsers(await api.users())
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Ошибка загрузки')
      } finally {
        setLoading(false)
      }
    }
    void load()
  }, [])

  return (
    <Card title="Пользователи">
      <Typography.Paragraph type="secondary">
        Цифровые двойники платформы: репутация, навыки и сделки.
      </Typography.Paragraph>
      <List
        loading={loading}
        dataSource={users}
        locale={{ emptyText: error ?? 'Пользователей пока нет' }}
        renderItem={(u) => (
          <List.Item>
            <List.Item.Meta
              avatar={<Avatar style={{ background: '#1677ff' }}>{u.name.charAt(0).toUpperCase()}</Avatar>}
              title={
                <>
                  {u.name} {!u.isActive && <Tag color="red">неактивен</Tag>}
                  <Tag color={u.role === 'admin' ? 'gold' : u.role === 'moderator' ? 'purple' : 'blue'}>
                    {u.role}
                  </Tag>
                </>
              }
              description={
                <>
                  {u.email}
                  {u.city ? ` • ${u.city}` : ''}
                  {' • '}
                  <Typography.Text type="secondary">TL{u.trustLevel}</Typography.Text>
                </>
              }
            />
          </List.Item>
        )}
      />
    </Card>
  )
}