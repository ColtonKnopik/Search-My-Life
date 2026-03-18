<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useJournalStore } from '@/stores/journalStore'
import { useAuthStore } from '@/stores/authStore'
import { encrypt, decrypt } from '@/services/cryptoService'

const route = useRoute()
const router = useRouter()
const journalStore = useJournalStore()
const authStore = useAuthStore()

const title = ref('')
const content = ref('')
const loading = ref(false)
const analyzing = ref(false)
const analyzeSuccess = ref(false)
const analyzeError = ref(null)
const error = ref(null)
const currentEntry = ref(null)

const isEditing = computed(() => !!route.params.id)
const pageTitle = computed(() => (isEditing.value ? 'Edit Entry' : 'New Entry'))
const needsAnalysis = computed(() => isEditing.value && currentEntry.value && !currentEntry.value.emotion)

// True when an encrypted entry exists but the password is not in memory (page was refreshed)
const isLocked = computed(() =>
  isEditing.value && currentEntry.value?.iv && !authStore.password
)

onMounted(async () => {
  if (isEditing.value) {
    const entry = journalStore.entries.find((e) => e.id === route.params.id)
    if (entry) {
      currentEntry.value = entry
      title.value = entry.title || ''

      if (entry.iv && entry.salt && authStore.password) {
        // Encrypted entry — decrypt for editing
        try {
          content.value = await decrypt(entry.content, entry.iv, entry.salt, authStore.password)
        } catch {
          error.value = 'Failed to decrypt this entry. Your password may have changed.'
        }
      } else {
        // Legacy plaintext entry (no IV) or unlocked session
        content.value = entry.content || ''
      }
    }
  }
})

async function handleReanalyze() {
  if (!content.value.trim()) return
  analyzing.value = true
  analyzeError.value = null
  analyzeSuccess.value = false
  try {
    await journalStore.analyzeEntry(route.params.id, content.value)
    analyzeSuccess.value = true
    currentEntry.value = journalStore.entries.find((e) => e.id === route.params.id)
  } catch {
    analyzeError.value = 'Re-analysis failed. Check that AI is configured.'
  } finally {
    analyzing.value = false
  }
}

async function handleSave() {
  if (!content.value.trim()) {
    error.value = 'Entry content cannot be empty.'
    return
  }

  loading.value = true
  error.value = null
  try {
    // Keep a reference to plaintext for AI analysis before encrypting
    const plaintext = content.value

    let entryContent = plaintext
    let iv = null
    let salt = null

    if (authStore.password) {
      const encrypted = await encrypt(plaintext, authStore.password)
      entryContent = encrypted.ciphertext
      iv = encrypted.iv
      salt = encrypted.salt
    }

    const entryData = { title: title.value, content: entryContent, iv, salt }
    let saved
    if (isEditing.value) {
      saved = await journalStore.updateEntry(route.params.id, entryData)
    } else {
      saved = await journalStore.createEntry(entryData)
    }

    // AI analysis always receives plaintext — never the ciphertext
    if (saved?.id && plaintext.trim()) {
      analyzing.value = true
      journalStore.analyzeEntry(saved.id, plaintext)
        .catch(() => {})
        .finally(() => { analyzing.value = false })
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

    <!-- Locked state: password not in memory after page refresh -->
    <template v-if="isLocked">
      <v-alert type="warning" icon="mdi-lock" class="mb-4">
        This entry is encrypted. Please log out and log back in to access it.
      </v-alert>
      <div class="d-flex justify-end">
        <v-btn color="primary" @click="router.back()">Go Back</v-btn>
      </div>
    </template>

    <template v-else>
      <v-alert v-if="error" type="error" class="mb-4" closable @click:close="error = null">
        {{ error }}
      </v-alert>

      <v-alert v-if="analyzeSuccess" type="success" class="mb-4" closable @click:close="analyzeSuccess = false">
        Entry analyzed — it will now appear in search results.
      </v-alert>

      <v-alert v-if="analyzeError" type="warning" class="mb-4" closable @click:close="analyzeError = null">
        {{ analyzeError }}
      </v-alert>

      <v-alert v-if="needsAnalysis" type="info" variant="tonal" class="mb-4" :icon="false">
        <div class="d-flex align-center justify-space-between flex-wrap gap-2">
          <span>This entry hasn't been analyzed yet and won't appear in search results.</span>
          <v-btn size="small" color="primary" :loading="analyzing" prepend-icon="mdi-brain" @click="handleReanalyze">
            Analyze Now
          </v-btn>
        </div>
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
    </template>
  </div>
</template>
