import { useState } from 'react'
import { Button, Card, Form, Input, Segmented, message } from 'antd'
import { LockOutlined, MailOutlined, UserOutlined } from '@ant-design/icons'
import { useAuth } from '../auth'

export default function LoginPage() {
  const { login, register } = useAuth()
  const [mode, setMode] = useState<'login' | 'register'>('login')
  const [loading, setLoading] = useState(false)
  const [form] = Form.useForm()

  const onFinish = async (values: { email: string; password: string; name?: string }) => {
    setLoading(true)
    try {
      if (mode === 'login') {
        await login(values.email, values.password)
      } else {
        await register(values.email, values.name ?? '', values.password)
      }
      message.success(mode === 'login' ? 'Вы вошли' : 'Аккаунт создан')
    } catch (err) {
      message.error(err instanceof Error ? err.message : 'Ошибка')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ maxWidth: 400, margin: '80px auto' }}>
      <Card title="AI Social Network">
        <Segmented
          block
          value={mode}
          onChange={(v) => {
            setMode(v as 'login' | 'register')
            form.resetFields()
          }}
          options={[
            { label: 'Вход', value: 'login' },
            { label: 'Регистрация', value: 'register' },
          ]}
          style={{ marginBottom: 24 }}
        />
        <Form form={form} layout="vertical" onFinish={onFinish} requiredMark={false}>
          {mode === 'register' && (
            <Form.Item name="name" label="Имя" rules={[{ required: true, message: 'Введите имя' }]}>
              <Input prefix={<UserOutlined />} placeholder="Ваше имя" />
            </Form.Item>
          )}
          <Form.Item
            name="email"
            label="Email"
            rules={[
              { required: true, message: 'Введите email' },
              { type: 'email', message: 'Некорректный email' },
            ]}
          >
            <Input prefix={<MailOutlined />} placeholder="you@example.com" />
          </Form.Item>
          <Form.Item name="password" label="Пароль" rules={[{ required: true, message: 'Введите пароль' }]}>
            <Input.Password prefix={<LockOutlined />} placeholder="••••••••" />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={loading}>
            {mode === 'login' ? 'Войти' : 'Создать аккаунт'}
          </Button>
        </Form>
      </Card>
    </div>
  )
}