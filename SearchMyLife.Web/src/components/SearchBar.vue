<script setup>
import { ref } from 'vue'

const emit = defineEmits(['search'])
const query = ref('')
let debounceTimer = null

function onInput() {
  clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => {
    emit('search', query.value)
  }, 400)
}

function onSubmit() {
  clearTimeout(debounceTimer)
  emit('search', query.value)
}
</script>

<template>
  <v-text-field
    v-model="query"
    label="Search your memories..."
    placeholder='Try: "When did I feel proud?" or "Stressful moments at work"'
    variant="outlined"
    prepend-inner-icon="mdi-brain"
    append-inner-icon="mdi-magnify"
    clearable
    rounded
    @input="onInput"
    @keyup.enter="onSubmit"
    @click:append-inner="onSubmit"
    @click:clear="query = ''"
  />
</template>
