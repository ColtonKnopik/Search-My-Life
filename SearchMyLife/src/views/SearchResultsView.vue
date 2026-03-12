<script setup>
import { useSearchStore } from '@/stores/searchStore'
import SearchBar from '@/components/SearchBar.vue'
import EntryCard from '@/components/EntryCard.vue'
import EmotionBadge from '@/components/EmotionBadge.vue'

const searchStore = useSearchStore()

function handleSearch(query) {
  if (query.trim()) {
    searchStore.search(query)
  }
}
</script>

<template>
  <div>
    <h1 class="text-h4 font-weight-bold mb-6">Search Your Memories</h1>

    <SearchBar @search="handleSearch" class="mb-6" />

    <v-progress-linear v-if="searchStore.isSearching" indeterminate color="primary" class="mb-4" />

    <v-alert v-if="searchStore.error" type="error" class="mb-4">
      {{ searchStore.error }}
    </v-alert>

    <div v-if="searchStore.query && !searchStore.isSearching && searchStore.results.length === 0" class="text-center py-12">
      <v-icon size="80" color="grey-lighten-1">mdi-magnify-close</v-icon>
      <h2 class="text-h5 mt-4 text-grey">No results found</h2>
      <p class="text-body-1 text-grey mt-2">Try a different question or phrase</p>
    </div>

    <div v-if="!searchStore.query && !searchStore.isSearching" class="text-center py-12">
      <v-icon size="80" color="grey-lighten-1">mdi-brain</v-icon>
      <h2 class="text-h5 mt-4 text-grey">Ask anything about your past</h2>
      <p class="text-body-1 text-grey mt-2">
        Try: "When did I last feel confident?" or "Times I was stressed about work"
      </p>
    </div>

    <v-row v-if="searchStore.results.length > 0">
      <v-col
        v-for="entry in searchStore.results"
        :key="entry.id"
        cols="12"
        md="6"
      >
        <EntryCard :entry="entry">
          <template #badges>
            <EmotionBadge v-if="entry.emotion" :emotion="entry.emotion" />
            <v-chip v-if="entry.score" size="small" variant="tonal" color="info">
              {{ Math.round(entry.score * 100) }}% match
            </v-chip>
          </template>
        </EntryCard>
      </v-col>
    </v-row>
  </div>
</template>
