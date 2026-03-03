<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const showPassword = ref(false)
const loading = ref(false)
const error = ref(null)

async function handleLogin() {
  loading.value = true
  error.value = null
  try {
    await authStore.login(email.value, password.value)
    router.push('/timeline')
  } catch (err) {
    error.value = err.response?.data?.message || 'Login failed. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <v-row justify="center" align="center" class="fill-height">
    <v-col cols="12" sm="8" md="5" lg="4">
      <v-card class="pa-6" elevation="8" rounded="lg">
        <v-card-title class="text-h4 text-center font-weight-bold pb-2">
          Welcome Back
        </v-card-title>
        <v-card-subtitle class="text-center pb-6">
          Sign in to Search My Life
        </v-card-subtitle>

        <v-alert v-if="error" type="error" class="mb-4" closable @click:close="error = null">
          {{ error }}
        </v-alert>

        <v-form @submit.prevent="handleLogin">
          <v-text-field
            v-model="email"
            label="Email"
            type="email"
            prepend-inner-icon="mdi-email-outline"
            variant="outlined"
            class="mb-2"
            required
          />
          <v-text-field
            v-model="password"
            label="Password"
            :type="showPassword ? 'text' : 'password'"
            prepend-inner-icon="mdi-lock-outline"
            :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
            variant="outlined"
            class="mb-4"
            required
            @click:append-inner="showPassword = !showPassword"
          />
          <v-btn
            type="submit"
            color="primary"
            size="large"
            block
            :loading="loading"
          >
            Sign In
          </v-btn>
        </v-form>

        <v-divider class="my-6" />

        <div class="text-center">
          <span class="text-body-2">Don't have an account?</span>
          <router-link to="/register" class="text-primary text-body-2 font-weight-bold ml-1">
            Sign Up
          </router-link>
        </div>
      </v-card>
    </v-col>
  </v-row>
</template>
