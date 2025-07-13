import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '../../stores/AuthStore'
import { mockUser, mockUserApiResponse } from '../mocks/api'

// Setup MSW server - removed since using direct API mocking

// Mock router
const mockPush = vi.fn()
const mockRouter = {
  push: mockPush,
  currentRoute: { value: { query: { redirect: undefined as string | undefined } } },
}

vi.mock('vue-router', () => ({
  useRouter: () => mockRouter,
}))

// Mock Api service to control authentication state
vi.mock('../../services/Api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}))

import { default as Api } from '../../services/Api'
const mockApi = Api as any

describe('AuthStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
    mockPush.mockClear()

    // Reset API mocks
    mockApi.get.mockReset()
    mockApi.post.mockReset()

    // Reset router state
    mockRouter.currentRoute.value.query.redirect = undefined
  })

  describe('Initial State', () => {
    it('should have correct initial state', () => {
      const authStore = useAuthStore()

      expect(authStore.user).toBeNull()
      expect(authStore.userId).toBeNull()
      expect(authStore.isLoggedIn).toBe(false)
      expect(authStore.isLoading).toBe(false)
      expect(authStore.errorMessage).toBeNull()
      expect(authStore.userChecked).toBe(false)
      expect(authStore.isAuthenticating).toBe(false)
    })

    it('should have correct initial form data', () => {
      const authStore = useAuthStore()

      expect(authStore.loginFormData).toEqual({
        email: '',
        password: '',
        rememberMe: false,
      })

      expect(authStore.registerFormData).toEqual({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: '',
      })
    })
  })

  describe('Computed Properties', () => {
    it('should compute isLoggedIn correctly', () => {
      const authStore = useAuthStore()

      expect(authStore.isLoggedIn).toBe(false)

      authStore.user = mockUser
      expect(authStore.isLoggedIn).toBe(true)
    })

    it('should compute form validation correctly', () => {
      const authStore = useAuthStore()

      // Invalid login form (empty)
      expect(authStore.isLoginFormValid).toBeFalsy()

      // Valid login form
      authStore.loginFormData.email = 'test@example.com'
      authStore.loginFormData.password = 'password123'
      expect(authStore.isLoginFormValid).toBe(true)

      // Invalid signup form (empty)
      expect(authStore.isSignupFormValid).toBeFalsy()

      // Valid signup form
      authStore.registerFormData.firstName = 'John'
      authStore.registerFormData.lastName = 'Doe'
      authStore.registerFormData.email = 'john@example.com'
      authStore.registerFormData.password = 'password123'
      authStore.registerFormData.confirmPassword = 'password123'
      expect(authStore.isSignupFormValid).toBe(true)
    })

    it('should detect password mismatch', () => {
      const authStore = useAuthStore()

      authStore.registerFormData.password = 'password123'
      authStore.registerFormData.confirmPassword = 'different123'
      expect(authStore.passwordMismatch).toBe(true)

      authStore.registerFormData.confirmPassword = 'password123'
      expect(authStore.passwordMismatch).toBe(false)
    })
  })

  describe('Actions', () => {
    describe('getCurrentUser', () => {
      it('should fetch current user successfully', async () => {
        const authStore = useAuthStore()

        // Mock successful API response
        mockApi.get.mockResolvedValueOnce({
          data: mockUserApiResponse,
        })

        const result = await authStore.getCurrentUser()

        expect(result).toBe(true)
        expect(authStore.user).toEqual(mockUserApiResponse)
        expect(authStore.userId).toBe(mockUserApiResponse.id)
        expect(authStore.userChecked).toBe(true)
        expect(authStore.isAuthenticating).toBe(false)
        expect(mockApi.get).toHaveBeenCalledWith('/api/user', { withCredentials: true })
      })

      it('should handle authentication failure', async () => {
        const authStore = useAuthStore()

        // Mock API failure
        mockApi.get.mockRejectedValueOnce(new Error('Unauthorized'))

        const result = await authStore.getCurrentUser()

        expect(result).toBe(false)
        expect(authStore.user).toBeNull()
        expect(authStore.userChecked).toBe(false)
      })

      it('should return early if already authenticating', async () => {
        const authStore = useAuthStore()
        authStore.isAuthenticating = true
        authStore.user = mockUser

        const result = await authStore.getCurrentUser()

        expect(result).toBe(true)
      })

      it('should return early if user already checked', async () => {
        const authStore = useAuthStore()
        authStore.userChecked = true
        authStore.user = mockUser

        const result = await authStore.getCurrentUser()

        expect(result).toBe(true)
      })
    })

    describe('handleLogin', () => {
      it('should login successfully', async () => {
        const authStore = useAuthStore()
        authStore.loginFormData.email = 'test@example.com'
        authStore.loginFormData.password = 'password123'

        // Mock successful login and user fetch
        mockApi.post.mockResolvedValueOnce({
          data: { message: 'Login successful' },
        })
        mockApi.get.mockResolvedValueOnce({
          data: mockUserApiResponse,
        })

        await authStore.handleLogin()

        expect(authStore.user).toEqual(mockUserApiResponse)
        expect(mockPush).toHaveBeenCalledWith({ name: 'books' })
        expect(authStore.loginFormData.email).toBe('')
        expect(authStore.loginFormData.password).toBe('')
        expect(mockApi.post).toHaveBeenCalledWith(
          '/api/login',
          {
            email: 'test@example.com',
            password: 'password123',
          },
          { withCredentials: true },
        )
      })

      it('should handle login with redirect', async () => {
        mockRouter.currentRoute.value.query.redirect = '/books'
        const authStore = useAuthStore()
        authStore.loginFormData.email = 'test@example.com'
        authStore.loginFormData.password = 'password123'

        // Mock successful login and user fetch
        mockApi.post.mockResolvedValueOnce({
          data: { message: 'Login successful' },
        })
        mockApi.get.mockResolvedValueOnce({
          data: mockUserApiResponse,
        })

        await authStore.handleLogin()

        expect(mockPush).toHaveBeenCalledWith('/books')
      })

      it('should handle login failure', async () => {
        const authStore = useAuthStore()
        authStore.loginFormData.email = 'test@example.com'
        authStore.loginFormData.password = 'wrong-password'

        // Mock API login failure
        mockApi.post.mockRejectedValueOnce({
          response: {
            status: 401,
            data: { message: 'Invalid credentials' },
          },
        })

        await authStore.handleLogin()

        expect(authStore.errorMessage).toBe('Invalid credentials')
        expect(authStore.user).toBeNull()
      })

      it('should not login with invalid form', async () => {
        const authStore = useAuthStore()

        await authStore.handleLogin()

        expect(authStore.errorMessage).toBe('Please enter a valid email and password.')
      })
    })

    describe('handleSignup', () => {
      it('should signup successfully', async () => {
        const authStore = useAuthStore()
        authStore.registerFormData.firstName = 'John'
        authStore.registerFormData.lastName = 'Doe'
        authStore.registerFormData.email = 'john@example.com'
        authStore.registerFormData.password = 'password123'
        authStore.registerFormData.confirmPassword = 'password123'

        // Mock successful signup
        mockApi.post.mockResolvedValueOnce({
          data: { message: 'Signup successful' },
        })

        await authStore.handleSignup()

        expect(mockPush).toHaveBeenCalledWith({ name: 'login' })
        expect(authStore.registerFormData.firstName).toBe('')
        expect(mockApi.post).toHaveBeenCalledWith('/api/signup', {
          name: 'John Doe',
          email: 'john@example.com',
          password: 'password123',
        })
      })

      it('should handle signup failure', async () => {
        const authStore = useAuthStore()
        authStore.registerFormData.firstName = 'John'
        authStore.registerFormData.lastName = 'Doe'
        authStore.registerFormData.email = 'john@example.com'
        authStore.registerFormData.password = 'password123'
        authStore.registerFormData.confirmPassword = 'password123'

        // Mock API signup failure
        mockApi.post.mockRejectedValueOnce({
          response: {
            status: 400,
            data: { message: 'User already exists' },
          },
        })

        await authStore.handleSignup()

        expect(authStore.errorMessage).toBe('User already exists')
      })

      it('should not signup with invalid form', async () => {
        const authStore = useAuthStore()

        await authStore.handleSignup()

        expect(authStore.errorMessage).toBe('Please fill in all fields correctly.')
      })
    })

    describe('logout', () => {
      it('should logout successfully', async () => {
        const authStore = useAuthStore()
        authStore.user = mockUser

        // Mock successful logout
        mockApi.post.mockResolvedValueOnce({
          data: { message: 'Logout successful' },
        })

        await authStore.logout()

        expect(authStore.user).toBeNull()
        expect(mockPush).toHaveBeenCalledWith({ name: 'login' })
        expect(mockApi.post).toHaveBeenCalledWith('/api/logout', {}, { withCredentials: true })
      })
    })

    describe('Helper Methods', () => {
      it('should reset form data', () => {
        const authStore = useAuthStore()
        authStore.loginFormData.email = 'test@example.com'
        authStore.registerFormData.firstName = 'John'

        authStore.resetFormData()

        expect(authStore.loginFormData.email).toBe('')
        expect(authStore.registerFormData.firstName).toBe('')
      })

      it('should clear auth state', () => {
        const authStore = useAuthStore()
        authStore.user = mockUser
        authStore.userId = '1'
        authStore.userChecked = true

        authStore.clearAuthState()

        expect(authStore.user).toBeNull()
        expect(authStore.userId).toBeNull()
        expect(authStore.userChecked).toBe(false)
      })
    })
  })
})
