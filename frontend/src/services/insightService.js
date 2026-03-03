import apiClient from '@/services/apiClient'

export async function generateReflection(timeRange) {
  const response = await apiClient.post('/insights/reflection', { timeRange })
  return response.data
}

export async function getEmotionTrends(timeRange) {
  const response = await apiClient.get('/insights/trends', {
    params: { timeRange },
  })
  return response.data
}
