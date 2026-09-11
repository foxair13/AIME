export interface AuthResult {
  token: string
  userId: number
  email: string
  name: string
  role: string
}

export interface User {
  id: number
  email: string
  name: string
  role: string
  bio: string | null
  city: string | null
  country: string | null
  isActive: boolean
  trustLevel: number
  createdAt: string
}

export interface RegisterInput {
  email: string
  name: string
  password: string
}

export interface LoginInput {
  email: string
  password: string
}

export const TOKEN_KEY = 'ai_social_network_token'

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token: string | null): void {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token)
  } else {
    localStorage.removeItem(TOKEN_KEY)
  }
}

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = getToken()
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string>),
  }
  if (token) {
    headers.Authorization = `Bearer ${token}`
  }

  const res = await fetch(`/api${path}`, { ...options, headers })

  if (res.status === 401) {
    setToken(null)
    throw new Error('Сессия истекла, войдите снова')
  }
  if (!res.ok) {
    let message = `Ошибка ${res.status}`
    try {
      const body = await res.json()
      if (body.title) message = body.title
      if (body.errors) {
        const first = Object.values(body.errors)[0]
        if (Array.isArray(first)) message = first.join('; ')
      }
    } catch {
      /* ignore */
    }
    throw new Error(message)
  }
  return res.json() as Promise<T>
}

export interface SearchHit {
  id: number
  type: string
  title: string
  summary: string | null
  score: number
}

export interface NotificationItem {
  id: number
  userId: number
  type: string
  payloadJson: string
  channel: string
  read: boolean
  createdAt: string
}

export const api = {
  register: (input: RegisterInput) =>
    request<AuthResult>('/auth/register', { method: 'POST', body: JSON.stringify(input) }),
  login: (input: LoginInput) =>
    request<AuthResult>('/auth/login', { method: 'POST', body: JSON.stringify(input) }),
  me: () => request<User>('/auth/me'),
  users: () => request<User[]>('/users'),
  user: (id: number) => request<User>(`/users/${id}`),
  search: (q: string, type = 'users') =>
    request<SearchHit[]>(`/search?q=${encodeURIComponent(q)}&type=${type}`),
  trending: () => request<SearchHit[]>('/search/trending'),
  notifications: (userId: number) =>
    request<NotificationItem[]>(`/notifications/unread/${userId}`),
  notificationsAll: (userId: number) =>
    request<NotificationItem[]>(`/notifications/${userId}`),
  markNotificationRead: (id: number) =>
    request<void>(`/notifications/${id}/read`, { method: 'POST' }),
  createNotification: (userId: number, type: string, payloadJson: string, channel = 'inapp') =>
    request<NotificationItem>('/notifications', {
      method: 'POST',
      body: JSON.stringify({ userId, type, payloadJson, channel }),
    }),
}