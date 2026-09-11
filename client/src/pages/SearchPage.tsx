import { useEffect, useState } from 'react'
import { Button, Card, Input, Result, Select, Space, Tag, Typography } from 'antd'
import { SearchOutlined } from '@ant-design/icons'
import { api, type SearchHit } from '../api'

const TYPES = [
  { label: 'Люди', value: 'users' },
  { label: 'Навыки', value: 'skills' },
  { label: 'Посты', value: 'posts' },
  { label: 'Группы', value: 'groups' },
  { label: 'Хэштеги', value: 'hashtags' },
]

const typeColor: Record<string, string> = {
  user: 'blue',
  skill: 'green',
  post: 'purple',
  group: 'orange',
  hashtag: 'cyan',
}

export default function SearchPage() {
  const [q, setQ] = useState('')
  const [type, setType] = useState('users')
  const [results, setResults] = useState<SearchHit[]>([])
  const [trending, setTrending] = useState<SearchHit[]>([])
  const [loading, setLoading] = useState(false)
  const [searched, setSearched] = useState(false)

  useEffect(() => {
    api.trending().then(setTrending).catch(() => setTrending([]))
  }, [])

  const doSearch = async () => {
    if (!q.trim()) return
    setLoading(true)
    setSearched(true)
    try {
      setResults(await api.search(q, type))
    } catch {
      setResults([])
    } finally {
      setLoading(false)
    }
  }

  return (
    <Card title="Поиск">
      <Space.Compact style={{ width: '100%' }}>
        <Input
          size="large"
          prefix={<SearchOutlined />}
          placeholder="Что ищем?"
          value={q}
          onChange={(e) => setQ(e.target.value)}
          onPressEnter={doSearch}
        />
        <Select size="large" value={type} onChange={setType} options={TYPES} style={{ width: 160 }} />
        <Button size="large" type="primary" icon={<SearchOutlined />} onClick={doSearch} loading={loading}>
          Найти
        </Button>
      </Space.Compact>

      {trending.length > 0 && !searched && (
        <div style={{ marginTop: 24 }}>
          <Typography.Text type="secondary">Тренды:</Typography.Text>
          <div style={{ marginTop: 8 }}>
            {trending.map((t) => (
              <Tag
                key={t.id}
                color="cyan"
                style={{ cursor: 'pointer' }}
                onClick={() => {
                  setQ(t.title.replace('#', ''))
                  setType('hashtags')
                  void doSearch()
                }}
              >
                {t.title} · {t.summary}
              </Tag>
            ))}
          </div>
        </div>
      )}

      {searched && results.length === 0 && !loading && (
        <Result status="info" title="Ничего не найдено" style={{ marginTop: 24 }} />
      )}

      <div style={{ marginTop: 24 }}>
        {results.map((r) => (
          <Card key={`${r.type}-${r.id}`} size="small" style={{ marginBottom: 8 }}>
            <Tag color={typeColor[r.type]}>{r.type}</Tag>
            <Typography.Text strong>{r.title}</Typography.Text>
            {r.summary && (
              <div>
                <Typography.Text type="secondary">{r.summary}</Typography.Text>
              </div>
            )}
          </Card>
        ))}
      </div>
    </Card>
  )
}