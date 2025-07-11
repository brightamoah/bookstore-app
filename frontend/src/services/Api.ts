import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8000'

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

// Request interceptor (optional - for logging in development)
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
  (error) => {
    if (error.response?.status === 401) {
      // Handle unauthorized access
      window.location.href = '/auth/login'
    }
    return Promise.reject(error)
  },
)

export default api
