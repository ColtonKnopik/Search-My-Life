<script setup>
import { computed } from 'vue'
import { getEmotionMeta } from '@/utils/emotionColors'

const props = defineProps({
  emotion: {
    type: String,
    required: true,
  },
})

const meta = computed(() => getEmotionMeta(props.emotion))
</script>

<template>
  <v-chip
    :color="meta.color"
    :prepend-icon="meta.icon"
    size="small"
    variant="tonal"
    label
  >
    {{ meta.label }}
  </v-chip>
</template>
