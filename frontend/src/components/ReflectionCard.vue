<script setup>
defineProps({
  reflection: {
    type: Object,
    required: true,
  },
})
</script>

<template>
  <div class="reflection-card">
    <p v-if="reflection.text" class="text-body-1 mb-3" style="white-space: pre-wrap;">
      {{ reflection.text }}
    </p>
    <p v-if="reflection.period" class="text-caption text-medium-emphasis">
      Based on entries from {{ reflection.period }}
    </p>
  </div>
</template>
