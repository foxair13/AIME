import { Avatar, Card, Col, Descriptions, Row, Statistic, Tag, Typography } from 'antd'
import { useAuth } from '../auth'

export default function ProfilePage() {
  const { user } = useAuth()

  if (!user) return null

  return (
    <Row gutter={16}>
      <Col span={10}>
        <Card title="Профиль" style={{ marginBottom: 16 }}>
          <div style={{ display: 'flex', gap: 16, alignItems: 'center', marginBottom: 16 }}>
            <Avatar size={64} style={{ background: '#1677ff', fontSize: 28 }}>
              {user.name.charAt(0).toUpperCase()}
            </Avatar>
            <div>
              <h2 style={{ margin: 0 }}>{user.name}</h2>
              <Tag color={user.role === 'admin' ? 'gold' : user.role === 'moderator' ? 'purple' : 'blue'}>
                {user.role}
              </Tag>
            </div>
          </div>
          <Descriptions column={1} size="small">
            <Descriptions.Item label="Email">{user.email}</Descriptions.Item>
            <Descriptions.Item label="Город">{user.city || '—'}</Descriptions.Item>
            <Descriptions.Item label="Страна">{user.country || '—'}</Descriptions.Item>
            <Descriptions.Item label="О себе">{user.bio || '—'}</Descriptions.Item>
            <Descriptions.Item label="Статус">
              {user.isActive ? <Tag color="green">активен</Tag> : <Tag color="red">заблокирован</Tag>}
            </Descriptions.Item>
          </Descriptions>
        </Card>
      </Col>
      <Col span={14}>
        <Card title="Репутация">
          <Row gutter={16}>
            <Col span={8}>
              <Statistic title="Уровень доверия" value={user.trustLevel} />
            </Col>
            <Col span={8}>
              <Statistic title="Сделки" value={0} />
            </Col>
            <Col span={8}>
              <Statistic title="Навыки" value={0} />
            </Col>
          </Row>
          <Typography.Paragraph type="secondary" style={{ marginTop: 24 }}>
            Счета сделок и навыков появятся после подключения ленты репутации и дерева компетенций.
          </Typography.Paragraph>
        </Card>
      </Col>
    </Row>
  )
}