import { ref } from 'vue'
import { defineStore } from 'pinia'
import apiClient from '@/services/apiClient'

export const useJournalStore = defineStore('journal', () => {
  const entries = ref([])
  const loading = ref(false)
  const error = ref(null)

  async function fetchEntries() {
    loading.value = true
    error.value = null
    try {
      const response = await apiClient.get('/entries')
      entries.value = response.data
    } catch (err) {
      error.value = err.message
    } finally {
      loading.value = false
    }
  }

  async function createEntry(entryData) {
    loading.value = true
    error.value = null
    try {
      const response = await apiClient.post('/entries', entryData)
      entries.value.unshift(response.data)
      return response.data
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateEntry(id, entryData) {
    loading.value = true
    error.value = null
    try {
      const response = await apiClient.put(`/entries/${id}`, entryData)
      const index = entries.value.findIndex((e) => e.id === id)
      if (index !== -1) {
        entries.value[index] = response.data
      }
      return response.data
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteEntry(id) {
    loading.value = true
    error.value = null
    try {
      await apiClient.delete(`/entries/${id}`)
      entries.value = entries.value.filter((e) => e.id !== id)
    } catch (err) {
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function analyzeEntry(id, plaintext) {
    try {
      const response = await apiClient.post(`/entries/${id}/analyze`, { plaintext })
      const index = entries.value.findIndex((e) => e.id === id)
      if (index !== -1) {
        entries.value[index] = { ...entries.value[index], ...response.data }
      }
      return response.data
    } catch (err) {
      // Analysis failure is non-critical — entry is already saved
      console.warn('AI analysis failed:', err.message)
    }
  }

  return { entries, loading, error, fetchEntries, createEntry, updateEntry, deleteEntry, analyzeEntry }
})
