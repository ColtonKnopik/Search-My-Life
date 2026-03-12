import { ref } from 'vue'
import { defineStore } from 'pinia'
import apiClient from '@/services/apiClient'

export const useSearchStore = defineStore('search', () => {
  const query = ref('')
  const results = ref([])
  const isSearching = ref(false)
  const error = ref(null)

  async function search(searchQuery) {
    query.value = searchQuery
    isSearching.value = true
    error.value = null
    try {
      const response = await apiClient.post('/search', { query: searchQuery })
      results.value = response.data
    } catch (err) {
      error.value = err.message
    } finally {
      isSearching.value = false
    }
  }

  function clearResults() {
    query.value = ''
    results.value = []
  }

  return { query, results, isSearching, error, search, clearResults }
})
