<script setup>
import { ref } from 'vue'

defineProps({
  entry: {
    type: Object,
    required: true,
  },
})

const expanded = ref(false)
</script>

<template>
  <v-card
    class="entry-card"
    elevation="2"
    rounded="lg"
    hover
    @click="expanded = !expanded"
  >
    <v-card-item>
      <v-card-title class="text-subtitle-1 font-weight-bold">
        {{ entry.title || 'Untitled Entry' }}
      </v-card-title>
      <v-card-subtitle>
        {{ new Date(entry.createdAt).toLocaleDateString('default', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }) }}
      </v-card-subtitle>
    </v-card-item>

    <v-card-text>
      <div class="d-flex flex-wrap ga-2 mb-3">
        <slot name="badges" />
        <v-chip
          v-for="tag in entry.tags"
          :key="tag"
          size="small"
          variant="outlined"
        >
          {{ tag }}
        </v-chip>
      </div>
      <p v-if="entry.summary" class="text-body-2 text-medium-emphasis">
        {{ entry.summary }}
      </p>
    </v-card-text>

    <v-expand-transition>
      <div v-show="expanded">
        <v-divider />
        <v-card-text>
          <p class="text-body-1" style="white-space: pre-wrap;">{{ entry.content }}</p>
          <div v-if="entry.sentimentScore != null" class="mt-4">
            <span class="text-caption text-medium-emphasis">
              Sentiment: {{ entry.sentimentScore > 0 ? '+' : '' }}{{ entry.sentimentScore.toFixed(2) }}
            </span>
          </div>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            size="small"
            variant="text"
            :to="`/entry/${entry.id}`"
            @click.stop
          >
            Edit
          </v-btn>
        </v-card-actions>
      </div>
    </v-expand-transition>
  </v-card>
</template>
