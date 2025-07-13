import axios, { AxiosError } from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080'

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const getImageUrl = (imagePath: string): string => {
  if (!imagePath) return ''

  if (imagePath.startsWith('http')) {
    return imagePath
  }

  return `${API_BASE_URL}/${imagePath}`
}

// Request interceptor - for logging in development
api.interceptors.request.use(
  (config) => {
    if (import.meta.env.DEV) {
      console.log('API Request:', config.method?.toUpperCase(), config.url)
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const { useAuthStore } = await import('@/stores/AuthStore')
    const { useBookStore } = await import('@/stores/BookStore')
    const { useSearchStore } = await import('@/stores/SearchStore')
    const authStore = useAuthStore()
    const bookStore = useBookStore()
    const searchStore = useSearchStore()

    if (error.response?.status === 401) {
      const errorData = error.response?.data as any
      const errorCode = errorData?.errorCode || errorData?.code

      console.log(`Unauthorized access (${error.response.status}):`, {
        errorCode,
        url: error.config?.url,
        data: errorData,
        message: error.message,
      })

      const isTokenExpired =
        errorCode === 'AUTH_003' ||
        errorCode === 'TOKEN_EXPIRED' ||
        errorData?.message?.includes('expired') ||
        errorData?.message?.includes('invalid token')

      const requiresAuth =
        error.config?.url?.includes('/api/books') || error.config?.url?.includes('/api/user')

      if (requiresAuth && !authStore.isLoggedIn && !authStore.user) {
        console.log('User not authenticated - redirecting to login')
        const router = await import('@/router')
        router.default.push({
          name: 'login',
          query: {
            redirect: window.location.pathname,
            message: 'Please log in to access this content',
          },
        })
        return Promise.reject(error)
      }

      if ((authStore.isLoggedIn || authStore.user) && isTokenExpired) {
        console.log('Handling authentication error - logging out user')
        try {
          await authStore.logout()
          bookStore.clearBookState()
          searchStore.clearSearchState()
        } catch (logoutError) {
          console.error('Error during logout:', logoutError)

          authStore.clearAuthState()
          bookStore.clearBookState()
          searchStore.clearSearchState()

          const router = await import('@/router')
          router.default.push({
            name: 'login',
            query: {
              expired: 'true',
              redirect: window.location.pathname,
            },
          })
        }
      } else if (isTokenExpired) {
        console.log('Token expired for unauthenticated user - redirecting to login')
        const toast = useToast()
        toast.add({
          title: 'Session Expired',
          description: 'Your session has expired. Please log in again.',
          color: 'error',
          close: true,
          duration: 10000,
        })
        const router = await import('@/router')
        router.default.push({
          name: 'login',
          query: {
            expired: 'true',
            redirect: window.location.pathname,
          },
        })
      } else {
        console.log('Unauthorized access - checking authentication state')
        if (authStore.isLoggedIn || authStore.user) {
          console.log('Clearing stale authentication state')
          authStore.clearAuthState()
          bookStore.clearBookState()
          searchStore.clearSearchState()

          const router = await import('@/router')
          router.default.push({
            name: 'login',
            query: {
              redirect: window.location.pathname,
              message: 'Your session has expired. Please log in again.',
            },
          })
        }
      }

      // Handle unauthorized access
      // window.location.href = '/auth/login'
    }
    return Promise.reject(error)
  },
)

export default api
