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

    <div v-if="searchStore.query && !searchStore.isSearching && !searchStore.hasResults" class="text-center py-12">
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

    <!-- AI Overview -->
    <v-alert
      v-if="searchStore.overview"
      type="info"
      variant="tonal"
      icon="mdi-brain"
      class="mb-6"
    >
      {{ searchStore.overview }}
    </v-alert>

    <!-- Top 3 Featured Results -->
    <div v-if="searchStore.topResults.length > 0" class="mb-8">
      <h2 class="text-h6 font-weight-medium mb-4 d-flex align-center">
        <v-icon class="mr-2" color="primary">mdi-star</v-icon>
        Most Relevant
      </h2>
      <v-row>
        <v-col
          v-for="(entry, index) in searchStore.topResults"
          :key="entry.id"
          cols="12"
          md="4"
        >
          <EntryCard :entry="entry" :rank="index + 1" :relevance-reason="entry.relevanceReason">
            <template #badges>
              <EmotionBadge v-if="entry.emotion" :emotion="entry.emotion" />
              <v-chip size="small" variant="tonal" color="primary">
                {{ Math.round(entry.score * 100) }}% match
              </v-chip>
            </template>
          </EntryCard>
        </v-col>
      </v-row>
    </div>

    <!-- Other Results -->
    <div v-if="searchStore.otherResults.length > 0">
      <h2 class="text-h6 font-weight-medium mb-4 d-flex align-center">
        <v-icon class="mr-2">mdi-format-list-bulleted</v-icon>
        More Results
      </h2>
      <v-row>
        <v-col
          v-for="entry in searchStore.otherResults"
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
  </div>
</template>
