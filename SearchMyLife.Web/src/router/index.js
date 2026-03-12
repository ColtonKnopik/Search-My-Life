import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { requiresAuth: false },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/RegisterView.vue'),
      meta: { requiresAuth: false },
    },
    {
      path: '/timeline',
      name: 'timeline',
      component: () => import('@/views/TimelineView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/entry/new',
      name: 'entry-new',
      component: () => import('@/views/JournalEntryView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/entry/:id',
      name: 'entry-edit',
      component: () => import('@/views/JournalEntryView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/search',
      name: 'search',
      component: () => import('@/views/SearchResultsView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/',
      redirect: '/timeline',
    },
  ],
})

router.beforeEach((to) => {
  const authStore = useAuthStore()
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return { name: 'login' }
  }
  if (to.meta.requiresAuth === false && authStore.isAuthenticated) {
    return { name: 'timeline' }
  }
})

export default router
