<script setup>
import { ref, onMounted } from 'vue'
import { generateReflection, getEmotionTrends } from '@/services/insightService'
import ReflectionCard from '@/components/ReflectionCard.vue'

const timeRange = ref('month')
const reflection = ref(null)
const trends = ref(null)
const loadingReflection = ref(false)
const loadingTrends = ref(false)
const error = ref(null)

async function loadReflection() {
  loadingReflection.value = true
  try {
    reflection.value = await generateReflection(timeRange.value)
  } catch (err) {
    error.value = err.message
  } finally {
    loadingReflection.value = false
  }
}

async function loadTrends() {
  loadingTrends.value = true
  try {
    trends.value = await getEmotionTrends(timeRange.value)
  } catch (err) {
    error.value = err.message
  } finally {
    loadingTrends.value = false
  }
}

onMounted(() => {
  loadReflection()
  loadTrends()
})
</script>

<template>
  <div>
    <div class="d-flex justify-space-between align-center mb-6">
      <h1 class="text-h4 font-weight-bold">Insights</h1>
      <v-btn-toggle v-model="timeRange" mandatory color="primary" density="compact" rounded>
        <v-btn value="week">Week</v-btn>
        <v-btn value="month">Month</v-btn>
        <v-btn value="year">Year</v-btn>
      </v-btn-toggle>
    </div>

    <v-alert v-if="error" type="error" class="mb-4" closable @click:close="error = null">
      {{ error }}
    </v-alert>

    <v-row>
      <v-col cols="12" md="6">
        <v-card class="pa-6" elevation="2" rounded="lg">
          <v-card-title class="text-h6 pb-4">
            <v-icon class="mr-2">mdi-chart-line</v-icon>
            Emotional Trends
          </v-card-title>
          <v-progress-linear v-if="loadingTrends" indeterminate color="primary" />
          <div v-else-if="trends" class="chart-placeholder text-center py-8">
            <v-icon size="64" color="grey-lighten-1">mdi-chart-areaspline</v-icon>
            <p class="text-body-2 text-grey mt-2">Chart will be rendered here (Phase 5)</p>
          </div>
          <div v-else class="text-center py-8">
            <p class="text-body-2 text-grey">No trend data available yet</p>
          </div>
        </v-card>
      </v-col>

      <v-col cols="12" md="6">
        <v-card class="pa-6" elevation="2" rounded="lg">
          <v-card-title class="text-h6 pb-4">
            <v-icon class="mr-2">mdi-lightbulb-outline</v-icon>
            AI Reflection
          </v-card-title>
          <v-progress-linear v-if="loadingReflection" indeterminate color="primary" />
          <ReflectionCard v-else-if="reflection" :reflection="reflection" />
          <div v-else class="text-center py-8">
            <p class="text-body-2 text-grey">Write more entries to generate reflections</p>
          </div>
          <v-card-actions>
            <v-spacer />
            <v-btn
              variant="tonal"
              color="primary"
              :loading="loadingReflection"
              @click="loadReflection"
            >
              Regenerate
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>
