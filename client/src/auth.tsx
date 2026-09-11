import { createContext, useContext, useEffect, useState, type ReactNode } from 'react'
import { api, setToken, getToken, type User } from './api'

interface AuthState {
  user: User | null
  loading: boolean
  login: (email: string, password: string) => Promise<User>
  register: (email: string, name: string, password: string) => Promise<User>
  logout: () => void
  refresh: () => Promise<User | null>
}

const AuthContext = createContext<AuthState | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState<boolean>(true)

  const applyToken = (token: string) => {
    setToken(token)
  }

  useEffect(() => {
    const restore = async () => {
      if (!getToken()) {
        setLoading(false)
        return
      }
      try {
        const me = await api.me()
        setUser(me)
      } catch {
        setToken(null)
      } finally {
        setLoading(false)
      }
    }
    void restore()
  }, [])

  const login = async (email: string, password: string) => {
    const result = await api.login({ email, password })
    applyToken(result.token)
    const me = await api.me()
    setUser(me)
    return me
  }

  const register = async (email: string, name: string, password: string) => {
    const result = await api.register({ email, name, password })
    applyToken(result.token)
    const me = await api.me()
    setUser(me)
    return me
  }

  const logout = () => {
    setToken(null)
    setUser(null)
  }

  const refresh = async () => {
    try {
      const me = await api.me()
      setUser(me)
      return me
    } catch {
      setUser(null)
      return null
    }
  }

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout, refresh }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth(): AuthState {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}