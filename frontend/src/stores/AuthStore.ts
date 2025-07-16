import api from '@/services/Api'
import type { AuthData, User } from '@/Types/types'
import { defineStore, acceptHMRUpdate } from 'pinia'
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'

export const useAuthStore = defineStore(
  'AuthStore',
  () => {
    const userId = ref<string | null>(null)
    const user = ref<User | null>(null)
    const router = useRouter()
    const isLoading = ref<boolean>(false)
    const errorMessage = ref<string | null>(null)
    const showPassword = ref<boolean>(false)
    const showConfirmPassword = ref<boolean>(false)
    const userChecked = ref<boolean>(false)
    const isAuthenticating = ref<boolean>(false)

    const isLoggedIn = computed<boolean>(() => !!user.value)

    const loginFormData = ref<AuthData['loginFormData']>({
      email: '',
      password: '',
      rememberMe: false,
    })

    const registerFormData = ref<AuthData['registerFormData']>({
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      confirmPassword: '',
    })

    const passwordMismatch = computed(() => {
      return (
        registerFormData.value.password &&
        registerFormData.value.confirmPassword &&
        registerFormData.value.password !== registerFormData.value.confirmPassword
      )
    })

    const isSignupFormValid = computed(() => {
      return (
        registerFormData.value.firstName &&
        registerFormData.value.lastName &&
        registerFormData.value.email &&
        registerFormData.value.password &&
        registerFormData.value.confirmPassword &&
        registerFormData.value.password === registerFormData.value.confirmPassword &&
        registerFormData.value.password.length >= 8 &&
        registerFormData.value.confirmPassword.length >= 8 &&
        !passwordMismatch.value
      )
    })

    const isLoginFormValid = computed(() => {
      return (
        loginFormData.value.email &&
        loginFormData.value.password &&
        loginFormData.value.password.length >= 8
      )
    })

    const resetFormData = () => {
      loginFormData.value = {
        email: '',
        password: '',
        rememberMe: false,
      }

      registerFormData.value = {
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: '',
      }
      errorMessage.value = null
      showPassword.value = false
      showConfirmPassword.value = false
    }

    const clearAuthState = () => {
      user.value = null
      userId.value = null
      userChecked.value = false
      isAuthenticating.value = false
      resetFormData()
    }

    const getCurrentUser = async () => {
      if (isAuthenticating.value) {
        return !!user.value
      }

      if (userChecked.value) {
        return !!user.value
      }

      isAuthenticating.value = true

      try {
        const response = await api.get('/api/user', { withCredentials: true })
        userId.value = response.data.id
        user.value = response.data as User
        userChecked.value = true
        return true
      } catch (error) {
        console.log('User authentication failed:', error)
        clearAuthState()
        return false
      } finally {
        isAuthenticating.value = false
      }
    }

    const handleSignup = async () => {
      if (!isSignupFormValid.value) {
        errorMessage.value = 'Please fill in all fields correctly.'
        return
      }
      isLoading.value = true
      errorMessage.value = null

      try {
        const response = await api.post('/api/signup', {
          name: `${registerFormData.value.firstName} ${registerFormData.value.lastName}`,
          email: registerFormData.value.email,
          password: registerFormData.value.password,
        })

        resetFormData()
        router.push({ name: 'login' })
        console.log('Signup successful:', response.data)
      } catch (error: any) {
        console.error('Signup error:', error)

        // Better error handling
        if (error.response?.data?.message) {
          errorMessage.value = error.response.data.message
        } else if (error.response?.status === 500) {
          errorMessage.value = 'Server error occurred. Please try again later.'
        } else if (error.code === 'ERR_NETWORK' || error.message.includes('Network Error')) {
          errorMessage.value = 'Network error. Please check your connection and try again.'
        } else {
          errorMessage.value = 'Signup failed. Please try again.'
        }
      } finally {
        isLoading.value = false
      }
    }

    const handleLogin = async () => {
      if (!isLoginFormValid.value) {
        errorMessage.value = 'Please enter a valid email and password.'
        return
      }

      isLoading.value = true
      errorMessage.value = null

      try {
        const response = await api.post(
          '/api/login',
          {
            email: loginFormData.value.email,
            password: loginFormData.value.password,
          },
          { withCredentials: true },
        )
        const isAuthenticated = await getCurrentUser()

        if (!isAuthenticated) throw new Error('Failed to get user information after login')

        resetFormData()
        console.log('Login successful:', response.data)

        const redirect = router.currentRoute.value.query.redirect as string
        if (redirect && redirect !== '/auth/login') {
          router.push(redirect)
        } else {
          router.push({ name: 'books' })
        }
      } catch (error: any) {
        console.error('Login error:', error)

        // Better error handling
        if (error.response?.data?.message) {
          errorMessage.value = error.response.data.message
        } else if (error.response?.status === 500) {
          errorMessage.value = 'Server error occurred. Please try again later.'
        } else if (error.code === 'ERR_NETWORK' || error.message.includes('Network Error')) {
          errorMessage.value = 'Network error. Please check your connection and try again.'
        } else if (error.response?.status === 401) {
          errorMessage.value = 'Invalid email or password.'
        } else {
          errorMessage.value = 'Login failed. Please try again.'
        }
      } finally {
        isLoading.value = false
      }
    }

    const logout = async () => {
      try {
        await api.post('/api/logout', {}, { withCredentials: true })
        router.push({ name: 'login' })
      } catch (error) {
        console.log('Logout API call failed:', error)
      } finally {
        clearAuthState()
        router.push({ name: 'login' })
      }
    }

    return {
      userId,
      isLoggedIn,
      isLoading,
      errorMessage,
      showPassword,
      showConfirmPassword,
      loginFormData,
      registerFormData,
      isSignupFormValid,
      passwordMismatch,
      isLoginFormValid,
      user,
      userChecked,
      isAuthenticating,
      clearAuthState,
      resetFormData,
      getCurrentUser,
      handleSignup,
      handleLogin,
      logout,
    }
  },
  {
    persist: {
      pick: ['user', 'userChecked'],
    },
  },
)

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useAuthStore, import.meta.hot))
}
