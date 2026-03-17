import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import apiClient from '@/services/apiClient'

export const useSearchStore = defineStore('search', () => {
  const query = ref('')
  const overview = ref('')
  const topResults = ref([])
  const otherResults = ref([])
  const isSearching = ref(false)
  const error = ref(null)

  const hasResults = computed(() => topResults.value.length > 0 || otherResults.value.length > 0)

  async function search(searchQuery) {
    query.value = searchQuery
    isSearching.value = true
    error.value = null
    try {
      const response = await apiClient.post('/search', { query: searchQuery })
      overview.value = response.data.overview || ''
      topResults.value = response.data.topResults || []
      otherResults.value = response.data.otherResults || []
    } catch (err) {
      error.value = err.message
    } finally {
      isSearching.value = false
    }
  }

  function clearResults() {
    query.value = ''
    overview.value = ''
    topResults.value = []
    otherResults.value = []
  }

  return { query, overview, topResults, otherResults, hasResults, isSearching, error, search, clearResults }
})
