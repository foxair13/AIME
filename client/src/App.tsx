import { Layout, Menu, Spin, Typography, message } from 'antd'
import { BellOutlined, LogoutOutlined, SearchOutlined, TeamOutlined, UserOutlined } from '@ant-design/icons'
import { useState } from 'react'
import { AuthProvider, useAuth } from './auth'
import LoginPage from './pages/LoginPage'
import UsersPage from './pages/UsersPage'
import ProfilePage from './pages/ProfilePage'
import SearchPage from './pages/SearchPage'
import NotificationsPage from './pages/NotificationsPage'

const { Header, Content, Sider } = Layout

function Shell() {
  const { user, loading, logout } = useAuth()
  const [page, setPage] = useState<'users' | 'profile' | 'search' | 'notifications'>('users')

  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', marginTop: 120 }}>
        <Spin size="large" />
      </div>
    )
  }

  if (!user) {
    return <LoginPage />
  }

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider theme="light" breakpoint="lg">
        <div style={{ padding: 16, fontWeight: 700, fontSize: 16 }}>
          AI Social Network
        </div>
        <Menu
          mode="inline"
          selectedKeys={[page]}
          onClick={(e) => setPage(e.key as 'users' | 'profile' | 'search' | 'notifications')}
          items={[
            { key: 'search', icon: <SearchOutlined />, label: 'Поиск' },
            { key: 'users', icon: <TeamOutlined />, label: 'Пользователи' },
            { key: 'notifications', icon: <BellOutlined />, label: 'Уведомления' },
            { key: 'profile', icon: <UserOutlined />, label: 'Профиль' },
          ]}
        />
      </Sider>
      <Layout>
        <Header style={{ background: '#fff', display: 'flex', justifyContent: 'flex-end', alignItems: 'center', paddingInline: 24 }}>
          <Typography.Text style={{ marginRight: 16 }} strong>
            {user.name}
          </Typography.Text>
          <a
            onClick={() => {
              logout()
              message.success('Вы вышли')
            }}
          >
            <LogoutOutlined /> Выйти
          </a>
        </Header>
        <Content style={{ padding: 24 }}>
          {page === 'users' && <UsersPage />}
          {page === 'profile' && <ProfilePage />}
          {page === 'search' && <SearchPage />}
          {page === 'notifications' && <NotificationsPage />}
        </Content>
      </Layout>
    </Layout>
  )
}

export default function App() {
  return (
    <AuthProvider>
      <Shell />
    </AuthProvider>
  )
}