<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useTheme } from 'vuetify'
import { useAuthStore } from '@/stores/authStore'

const router = useRouter()
const theme = useTheme()
const authStore = useAuthStore()
const drawer = ref(true)

const isAuthenticated = computed(() => authStore.isAuthenticated)

const navItems = [
  { title: 'Timeline', icon: 'mdi-timeline-clock-outline', to: '/timeline' },
  { title: 'New Entry', icon: 'mdi-pencil-plus-outline', to: '/entry/new' },
  { title: 'Search', icon: 'mdi-magnify', to: '/search' },
]

function toggleTheme() {
  theme.global.name.value = theme.global.current.value.dark ? 'light' : 'dark'
}

function logout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <v-app>
    <!-- App bar -->
    <v-app-bar color="primary" prominent>
      <v-app-bar-nav-icon
        v-if="isAuthenticated"
        @click="drawer = !drawer"
      />
      <v-toolbar-title class="text-h5 font-weight-bold">
        Search My Life
      </v-toolbar-title>
      <v-spacer />
      <v-btn icon @click="toggleTheme">
        <v-icon>{{ theme.global.current.value.dark ? 'mdi-weather-sunny' : 'mdi-weather-night' }}</v-icon>
      </v-btn>
      <v-btn v-if="isAuthenticated" icon @click="logout">
        <v-icon>mdi-logout</v-icon>
      </v-btn>
    </v-app-bar>

    <!-- Navigation drawer (only when authenticated) -->
    <v-navigation-drawer
      v-if="isAuthenticated"
      v-model="drawer"
      app
    >
      <v-list nav>
        <v-list-item
          v-for="item in navItems"
          :key="item.title"
          :prepend-icon="item.icon"
          :title="item.title"
          :to="item.to"
          rounded="xl"
        />
      </v-list>
    </v-navigation-drawer>

    <!-- Main content -->
    <v-main>
      <v-container fluid>
        <RouterView />
      </v-container>
    </v-main>
  </v-app>
</template>
