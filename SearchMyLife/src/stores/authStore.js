import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import apiClient from '@/services/apiClient'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || null)
  const user = ref(parseStoredUser())
  // Password is kept in memory only — never persisted.
  // It is the key material for client-side AES-GCM encryption.
  const password = ref(null)

  function parseStoredUser() {
    try {
      return JSON.parse(localStorage.getItem('user') || 'null')
    } catch {
      localStorage.removeItem('user')
      localStorage.removeItem('token')
      return null
    }
  }

  const isAuthenticated = computed(() => !!token.value)

  async function login(email, pwd) {
    const response = await apiClient.post('/auth/login', { email, password: pwd })
    token.value = response.data.token
    user.value = response.data.user
    password.value = pwd
    localStorage.setItem('token', token.value)
    localStorage.setItem('user', JSON.stringify(user.value))
    return response.data
  }

  async function register(email, pwd) {
    const response = await apiClient.post('/auth/register', { email, password: pwd })
    token.value = response.data.token
    user.value = response.data.user
    password.value = pwd
    localStorage.setItem('token', token.value)
    localStorage.setItem('user', JSON.stringify(user.value))
    return response.data
  }

  function logout() {
    token.value = null
    user.value = null
    password.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  return { token, user, password, isAuthenticated, login, register, logout }
})
