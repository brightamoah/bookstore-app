import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import { useAuthStore } from '@/stores/AuthStore'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/books',
      name: 'books',
      component: () => import('../views/Books.vue'),
      meta: { requiresAuth: true },
    },
    {
      name: 'signup',
      path: '/auth/signup',
      component: () => import('../views/auth/Signup.vue'),
    },
    {
      name: 'login',
      path: '/auth/login',
      component: () => import('../views/auth/Login.vue'),
    },
    {
      name: 'book-details',
      component: () => import('../views/books/BookDetails.vue'),
      path: '/books/:id',
      meta: { requiresAuth: true },
    },
  ],
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  const authRoute = to.name === 'login' || to.name === 'signup'
  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth)

  // Check if the route requires authentication
  if (requiresAuth && !authStore.user) {
    next({ name: 'login', query: { redirect: to.fullPath } })
  } else if (authRoute && authStore.user) {
    next({ name: 'books' })
  } else {
    if (to.query.redirect && authStore.user) {
      next(to.query.redirect as string)
    } else {
      next()
    }
  }
})

export default router
