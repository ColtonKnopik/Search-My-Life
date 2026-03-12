import apiClient from '@/services/apiClient'

export async function searchEntries(query) {
  const response = await apiClient.post('/search', { query })
  return response.data
}
