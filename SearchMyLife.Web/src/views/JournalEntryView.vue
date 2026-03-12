<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useJournalStore } from '@/stores/journalStore'

const route = useRoute()
const router = useRouter()
const journalStore = useJournalStore()

const title = ref('')
const content = ref('')
const loading = ref(false)
const error = ref(null)

const isEditing = computed(() => !!route.params.id)
const pageTitle = computed(() => (isEditing.value ? 'Edit Entry' : 'New Entry'))

onMounted(async () => {
  if (isEditing.value) {
    const entry = journalStore.entries.find((e) => e.id === route.params.id)
    if (entry) {
      title.value = entry.title || ''
      content.value = entry.content || ''
    }
  }
})

async function handleSave() {
  if (!content.value.trim()) {
    error.value = 'Entry content cannot be empty.'
    return
  }

  loading.value = true
  error.value = null
  try {
    const entryData = {
      title: title.value,
      content: content.value,
    }
    if (isEditing.value) {
      await journalStore.updateEntry(route.params.id, entryData)
    } else {
      await journalStore.createEntry(entryData)
    }
    router.push('/timeline')
  } catch (err) {
    error.value = err.response?.data?.message || 'Failed to save entry.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div>
    <div class="d-flex align-center mb-6">
      <v-btn icon variant="text" class="mr-2" @click="router.back()">
        <v-icon>mdi-arrow-left</v-icon>
      </v-btn>
      <h1 class="text-h4 font-weight-bold">{{ pageTitle }}</h1>
    </div>

    <v-alert v-if="error" type="error" class="mb-4" closable @click:close="error = null">
      {{ error }}
    </v-alert>

    <v-card class="pa-6" elevation="2" rounded="lg">
      <v-form @submit.prevent="handleSave">
        <v-text-field
          v-model="title"
          label="Title (optional)"
          variant="outlined"
          class="mb-4"
          prepend-inner-icon="mdi-format-title"
        />
        <v-textarea
          v-model="content"
          label="What's on your mind?"
          variant="outlined"
          rows="12"
          auto-grow
          class="mb-4"
          prepend-inner-icon="mdi-text"
          required
        />
        <div class="d-flex justify-end gap-3">
          <v-btn variant="outlined" @click="router.back()">Cancel</v-btn>
          <v-btn type="submit" color="primary" :loading="loading" prepend-icon="mdi-content-save">
            Save Entry
          </v-btn>
        </div>
      </v-form>
    </v-card>
  </div>
</template>
