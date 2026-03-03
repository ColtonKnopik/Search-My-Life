<script setup>
import { computed, onMounted } from 'vue'
import { useJournalStore } from '@/stores/journalStore'
import EntryCard from '@/components/EntryCard.vue'
import EmotionBadge from '@/components/EmotionBadge.vue'

const journalStore = useJournalStore()

onMounted(() => {
  journalStore.fetchEntries()
})

const groupedEntries = computed(() => {
  const groups = {}
  for (const entry of journalStore.entries) {
    const date = new Date(entry.createdAt)
    const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`
    const label = date.toLocaleString('default', { month: 'long', year: 'numeric' })
    if (!groups[key]) {
      groups[key] = { key, label, entries: [] }
    }
    groups[key].entries.push(entry)
  }
  return Object.values(groups).sort((a, b) => b.key.localeCompare(a.key))
})
</script>

<template>
  <div>
    <div class="d-flex justify-space-between align-center mb-6">
      <h1 class="text-h4 font-weight-bold">Timeline</h1>
      <v-btn color="primary" prepend-icon="mdi-pencil-plus-outline" to="/entry/new">
        New Entry
      </v-btn>
    </div>

    <v-progress-linear v-if="journalStore.loading" indeterminate color="primary" class="mb-4" />

    <v-alert v-if="journalStore.error" type="error" class="mb-4">
      {{ journalStore.error }}
    </v-alert>

    <div v-if="!journalStore.loading && groupedEntries.length === 0" class="text-center py-12">
      <v-icon size="80" color="grey-lighten-1">mdi-notebook-outline</v-icon>
      <h2 class="text-h5 mt-4 text-grey">No entries yet</h2>
      <p class="text-body-1 text-grey mt-2">Start journaling to see your timeline</p>
      <v-btn color="primary" class="mt-4" to="/entry/new">Write Your First Entry</v-btn>
    </div>

    <div v-for="group in groupedEntries" :key="group.key" class="mb-8">
      <h2 class="text-h5 font-weight-medium mb-4 d-flex align-center">
        <v-icon class="mr-2">mdi-calendar-month</v-icon>
        {{ group.label }}
      </h2>
      <v-row>
        <v-col
          v-for="entry in group.entries"
          :key="entry.id"
          cols="12"
          md="6"
          lg="4"
        >
          <EntryCard :entry="entry">
            <template #badges>
              <EmotionBadge v-if="entry.emotion" :emotion="entry.emotion" />
            </template>
          </EntryCard>
        </v-col>
      </v-row>
    </div>
  </div>
</template>
