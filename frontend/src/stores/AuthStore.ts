import api from '@/services/Api'
import type { AuthData, User } from '@/Types/types'
import axios from 'axios'
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

    const getCurrentUser = async () => {
      if (userChecked.value && !user.value) {
        return
      }

      try {
        const response = await api.get('/api/user', { withCredentials: true })
        userId.value = response.data.id
        user.value = response.data as User
        userChecked.value = true
      } catch (error) {
        user.value = null
        userId.value = null
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
        const response = await axios.post('/api/signup', {
          name: `${registerFormData.value.firstName} ${registerFormData.value.lastName}`,
          email: registerFormData.value.email,
          password: registerFormData.value.password,
        })
        isLoading.value = false
        resetFormData()
        router.push({ name: 'login' })
        console.log('Signup successful:', response.data)
      } catch (error) {
        isLoading.value = false
        errorMessage.value = axios.isAxiosError(error)
          ? error.response?.data?.message
          : 'Signup failed. Please try again.'
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
        await getCurrentUser()

        // user.value = response.data.user as User
        // userId.value = response.data.user.id.toString()
        resetFormData()
        console.log('Login successful:', response.data)
        isLoading.value = false
        router.push({ name: 'books' })
      } catch (error) {
        isLoading.value = false
        errorMessage.value = axios.isAxiosError(error)
          ? error.response?.data?.message
          : 'Login failed. Please try again.'
      }
    }

    const logout = async () => {
      try {
        await api.post('/api/logout', {}, { withCredentials: true })
        user.value = null
        userChecked.value = false
        userId.value = null
        resetFormData()
        router.push({ name: 'login' })
      } catch (error) {
        errorMessage.value = axios.isAxiosError(error)
          ? error.response?.data?.message
          : 'Logout failed. Please try again.'
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
      handleSignup,
      handleLogin,
      resetFormData,
      getCurrentUser,
      logout,
    }
  },
  {
    persist: true,
  },
)

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useAuthStore, import.meta.hot))
}
