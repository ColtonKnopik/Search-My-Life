<script setup>
import { ref } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { decrypt } from '@/services/cryptoService'

const props = defineProps({
  entry: {
    type: Object,
    required: true,
  },
  rank: {
    type: Number,
    default: null,
  },
  relevanceReason: {
    type: String,
    default: null,
  },
})

const authStore = useAuthStore()
const expanded = ref(false)
const decryptedContent = ref(null)
const decryptError = ref(false)

async function toggleExpanded() {
  expanded.value = !expanded.value

  if (expanded.value && decryptedContent.value === null && !decryptError.value) {
    if (props.entry.iv && props.entry.salt) {
      if (!authStore.password) {
        decryptError.value = true
        return
      }
      try {
        decryptedContent.value = await decrypt(
          props.entry.content,
          props.entry.iv,
          props.entry.salt,
          authStore.password,
        )
      } catch {
        decryptError.value = true
      }
    } else {
      // Legacy plaintext entry — no IV
      decryptedContent.value = props.entry.content
    }
  }
}
</script>

<template>
  <v-card
    :class="['entry-card', { 'featured-card': rank !== null }]"
    :elevation="rank !== null ? 4 : 2"
    rounded="lg"
    hover
    :border="rank !== null"
    @click="toggleExpanded"
  >
    <v-card-item>
      <template v-if="rank !== null" #prepend>
        <v-avatar color="primary" variant="tonal" size="36" class="mr-2">
          <span class="text-body-1 font-weight-bold">{{ rank }}</span>
        </v-avatar>
      </template>
      <v-card-title class="text-subtitle-1 font-weight-bold">
        {{ entry.title || 'Untitled Entry' }}
      </v-card-title>
      <v-card-subtitle>
        {{ new Date(entry.createdAt).toLocaleDateString('default', { weekday: rank === null ? 'long' : undefined, year: 'numeric', month: 'long', day: 'numeric' }) }}
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
      <v-alert
        v-if="relevanceReason"
        density="compact"
        variant="tonal"
        color="primary"
        icon="mdi-lightbulb-outline"
        class="text-body-2 mt-3"
      >
        {{ relevanceReason }}
      </v-alert>
    </v-card-text>

    <v-expand-transition>
      <div v-show="expanded">
        <v-divider />
        <v-card-text>
          <v-alert v-if="decryptError" type="warning" density="compact" icon="mdi-lock">
            Log out and back in to view this entry's content.
          </v-alert>
          <p v-else class="text-body-1" style="white-space: pre-wrap;">{{ decryptedContent }}</p>
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

<style scoped>
.featured-card {
  border-color: rgb(var(--v-theme-primary)) !important;
  border-width: 1px;
}
</style>
